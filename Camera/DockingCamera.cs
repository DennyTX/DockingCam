using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 


namespace DockingCamera
{
    class DockingCamera:BaseKspCamera
    {
        private static List<Texture2D>[] textureWhiteNoise;
        private static GUIStyle guiStyleRedLabel;
        private static GUIStyle guiStyleGreenLabel; 

        private static HashSet<int> usedId = new HashSet<int>();

        private static float CurrentY = 64;

        private int ID;
        private int idTextureNoise; 

        private Texture2D textureVLineOLDD;
        private Texture2D textureHLineOLDD; 
        private Texture2D textureVLine;
        private Texture2D textureHLine;
        private Texture2D textureVLineBack;
        private Texture2D textureHLineBack;

        private GameObject moduleDockingNodeGameObject; 
        private TargetHelper target;

        private bool noiseActive;
        private bool cameraData = true;
        private bool rotatorState = true;
        private bool targetCrossDenny = false;
        private bool targetCrossDPAI = true;
        public float MaxSpeed = 3;

        private Color targetCrossColorOLDD = new Color(0, 0, 0.9f, 1);
        private Color targetCrossColor = new Color(0.5f, .0f, 0, 1);
        private Color targetCrossColorBack = new Color(.9f, 0, 0, 1);

        private string lastVesselName;
        private string windowLabelSuffix;
        public Color TargetCrossColorOLDD
        {
            get { return targetCrossColorOLDD; }
            set
            {
                targetCrossColorOLDD = value;
                textureVLineOLDD = Util.MonoColorVerticalLineTexture(targetCrossColorOLDD, (int)windowSize * windowSizeCoef);
                textureHLineOLDD = Util.MonoColorHorizontalLineTexture(targetCrossColorOLDD, (int)windowSize * windowSizeCoef);
            }
        }
        public Color TargetCrossColor
        {
            get { return targetCrossColor; }
            set
            {
                targetCrossColor = value;
                textureVLine = Util.MonoColorVerticalLineTexture(TargetCrossColor, (int)windowSize * windowSizeCoef);
                textureHLine = Util.MonoColorHorizontalLineTexture(TargetCrossColor, (int)windowSize * windowSizeCoef);
            }
        }
        public Color TargetCrossColorBack
        {
            get { return targetCrossColorBack; }
            set
            {
                targetCrossColorBack = value;
                textureVLineBack = Util.MonoColorVerticalLineTexture(TargetCrossColorBack, (int)windowSize * windowSizeCoef);
                textureHLineBack = Util.MonoColorHorizontalLineTexture(TargetCrossColorBack, (int)windowSize * windowSizeCoef);
            }
        }

        public DockingCamera(Part part, bool noiseActive, int windowSize, string windowLabel = "DockCam")
            : base(part, windowSize, windowLabel)
        {
            this.noiseActive = noiseActive;
            target = new TargetHelper(part);
            guiStyleRedLabel = new GUIStyle(HighLogic.Skin.label);
            guiStyleGreenLabel = new GUIStyle(HighLogic.Skin.label);
            guiStyleRedLabel.normal.textColor = Color.red;
            guiStyleGreenLabel.normal.textColor = Color.green;

            //GameEvents.onVesselChange.Add(vesselChange);

            moduleDockingNodeGameObject = partGameObject.GetChild("dockingNode") ?? partGameObject;  //GET orientation from dockingnode

            if (textureWhiteNoise != null || !noiseActive) return;
            textureWhiteNoise = new List<Texture2D>[3];
            for (int j = 0; j < 3; j++)
            {
                textureWhiteNoise[j] = new List<Texture2D>();
                for (int i = 0; i < 4; i++)
                    textureWhiteNoise[j].Add(Util.WhiteNoiseTexture((int)texturePosition.width, (int)texturePosition.height));
            }
        }

        //~DockingCamera()  //desctruction
        //{
        //    GameEvents.onVesselChange.Remove(vesselChange);
        //}

        //private void vesselChange(Vessel vessel)
        //{
        //    //if (TargetHelper.IsTargetSelect)
        //    //    windowLabelSuffix = TargetHelper.Target.GetName();
        //    //else
        //    //    windowLabelSuffix = " NO TARGET";
        //    windowLabel = subWindowLabel + " " + ID + " to " + lastVesselName;  //TargetHelper.Target.GetName(); //
        //}

        protected override void InitTextures()
        {
            base.InitTextures();
            textureVLineOLDD = Util.MonoColorVerticalLineTexture(TargetCrossColorOLDD, (int)windowSize * windowSizeCoef);
            textureHLineOLDD = Util.MonoColorHorizontalLineTexture(TargetCrossColorOLDD, (int)windowSize * windowSizeCoef); 
            textureVLine = Util.MonoColorVerticalLineTexture(TargetCrossColor, (int)windowSize * windowSizeCoef);
            textureHLine = Util.MonoColorHorizontalLineTexture(TargetCrossColor, (int)windowSize * windowSizeCoef);
            textureVLineBack = Util.MonoColorVerticalLineTexture(targetCrossColorBack, (int)windowSize * windowSizeCoef);
            textureHLineBack = Util.MonoColorHorizontalLineTexture(targetCrossColorBack, (int)windowSize * windowSizeCoef);
        }

        protected override void ExtendedDrawWindowL1()
        {
            var widthOffset = windowPosition.width - 86;
            cameraData = GUI.Toggle(new Rect(widthOffset, 34, 76, 20), cameraData, "Flight data");
            rotatorState = GUI.Toggle(new Rect(widthOffset, 54, 76, 20), rotatorState, "Rotator");
            targetCrossDPAI = GUI.Toggle(new Rect(widthOffset, 74, 76, 30), targetCrossDPAI, "Cross\n DPAI");
            targetCrossDenny = GUI.Toggle(new Rect(widthOffset, 106, 76, 30), targetCrossDenny, "Cross\n OLDD");
            noiseActive = GUI.Toggle(new Rect(widthOffset, 180, 76, 20), noiseActive, "Noise");
            base.ExtendedDrawWindowL1();
        }

        protected override void ExtendedDrawWindowL2()
        {
            GUI.DrawTexture(texturePosition, AssetLoader.texDockingCam);
            if (noiseActive)
                GUI.DrawTexture(texturePosition, textureWhiteNoise[windowSizeCoef-2][idTextureNoise]);  //whitenoise
            base.ExtendedDrawWindowL2();
        }

        protected override void ExtendedDrawWindowL3()
        {
            if (GUI.RepeatButton(new Rect(7, 33, 20, 20), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom;

            }
            if (GUI.RepeatButton(new Rect(26, 33, 20, 20), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }

            //LAMP&Seconds Block
            if (target.isMoveToTarget)
            {
                GUI.DrawTexture(new Rect(texturePosition.xMin + 20, texturePosition.yMax - 20, 20, 20),
                    AssetLoader.texLampOn);
                GUI.Label(new Rect(texturePosition.xMin + 40, texturePosition.yMax - 20, 140, 20),
                    String.Format("Time to dock:{0:f0}s", target.SecondsToDock));
            }
            else
                GUI.DrawTexture(new Rect(texturePosition.xMin + 20, texturePosition.yMax - 20, 20, 20),
                    AssetLoader.texLampOff);

            GetWindowLabel();
            GetFlightData();
            GetCross();

            if (rotatorState) // && TargetHelper.IsTargetSelect && part.vessel.Equals(FlightGlobals.ActiveVessel))
            {
                var size = texturePosition.width / 8  ;
                var x = texturePosition.xMin + texturePosition.width / 2 - size / 2;
                GUI.DrawTexture(new Rect(x, texturePosition.yMax - size, size, size), AssetLoader.texSelfRot);
                Matrix4x4 matrixBackup = GUI.matrix;
                var position = new Rect(x, texturePosition.yMax - size, size, size);
                GUIUtility.RotateAroundPivot(target.AngleZ, position.center);
                GUI.DrawTexture(position, AssetLoader.texTargetRot);
                GUI.matrix = matrixBackup;
            }

            base.ExtendedDrawWindowL3();
        }

        private void GetWindowLabel()
        {
            if (part.vessel.Equals(FlightGlobals.ActiveVessel))
                if (TargetHelper.IsTargetSelect) // && part.vessel.Equals(FlightGlobals.ActiveVessel))
                {
                    lastVesselName = TargetHelper.Target.GetName();
                    windowLabelSuffix = " to " + lastVesselName;
                    windowLabel = subWindowLabel + " " + ID + windowLabelSuffix;
                }
                else
                {
                    if (part.vessel.Equals(FlightGlobals.ActiveVessel))
                    {
                        windowLabel = subWindowLabel + " " + ID;
                        lastVesselName = "";
                        windowLabelSuffix = lastVesselName;
                    }
                }
            else
            {
                windowLabel = subWindowLabel + " " + ID + windowLabelSuffix;    
            }
            //if (!part.vessel.Equals(FlightGlobals.ActiveVessel))  //autoaim?
            //    windowLabel = subWindowLabel + " " + ID + windowLabelSuffix;  
           
        }

        private void GetCross()
        {
            if (targetCrossDPAI)
            {
                ////RotationXY Block
                var textV = target.LookForward ? textureVLine : textureVLineBack;
                var textH = target.LookForward ? textureHLine : textureHLineBack;
                var tx = target.TargetMoveHelpX;
                var ty = target.TargetMoveHelpY;
                if (!target.LookForward)
                {
                    tx = 1 - tx;
                    ty = 1 - ty;
                }
                GUI.DrawTexture(
                    new Rect(texturePosition.xMin + Math.Abs(tx*texturePosition.width)%texturePosition.width,
                        texturePosition.yMin,
                        1,
                        texturePosition.height),
                    textV);
                GUI.DrawTexture(
                    new Rect(texturePosition.xMin,
                        texturePosition.yMin + Math.Abs(ty*texturePosition.height)%texturePosition.height,
                        texturePosition.width,
                        1),
                    textH);
            }

            if (targetCrossDenny)
            {
                ////RotationXY Block
                var tx = texturePosition.width/2;
                var ty = texturePosition.height/2;
                if (Mathf.Abs(target.AngleX) > 20)
                    tx += (target.AngleX > 0 ? -1 : 1)*(texturePosition.width/2 - 1);
                else
                    tx += (texturePosition.width/40)*-target.AngleX;
                if (Mathf.Abs(target.AngleY) > 20)
                    ty += (target.AngleY > 0 ? -1 : 1)*(texturePosition.height/2 - 1);
                else
                    ty += (texturePosition.height/40)*-target.AngleY;

                GUI.DrawTexture(
                    new Rect(texturePosition.xMin + tx, texturePosition.yMin, 1, texturePosition.height),
                    textureVLineOLDD);
                GUI.DrawTexture(
                    new Rect(texturePosition.xMin, texturePosition.yMin + ty, texturePosition.width, 1),
                    textureHLineOLDD);
            }
        }

        private void GetFlightData()
        {
            if (cameraData)
            {
                if (TargetHelper.IsTargetSelect && part.vessel.Equals(FlightGlobals.ActiveVessel))
                {
                    /// DATA block
                    /// <summary>
                    float i = 0;
                    target.Update();

                    if (!target.isDockPort)
                    {
                        GUI.Label(new Rect(texturePosition.xMin + 10, 54, 100, 40),
                            "Target is not\n a DockPort");
                        if (target.Destination < 200f)
                            GUI.Label(new Rect(texturePosition.xMin + 10, 84, 96, 40),
                                "DockPort is\n available", guiStyleGreenLabel);
                    }

                    /// <summary>
                    /// FlightDATA
                    /// <summary>
                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("Dist:{0:f2}", target.Destination));
                    i += .2f;

                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("dx:{0:f2}", target.DX));
                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("dy:{0:f2}", target.DY));
                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("dz:{0:f2}", target.DZ));
                    i += .2f;

                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("vX:{0:f2}", target.SpeedX));
                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("vY:{0:f2}", target.SpeedY));
                    if (target.SpeedZ < -MaxSpeed)
                        GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                            String.Format("vZ:{0:f2}", target.SpeedZ), guiStyleRedLabel);
                    else
                        GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                            String.Format("vZ:{0:f2}", target.SpeedZ));
                    i += .2f;

                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("Yaw:{0:f0}°", target.AngleX));
                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("Pitch:{0:f0}°", target.AngleY));
                    GUI.Label(new Rect(texturePosition.xMax - 70, 32 + i++*20, 70, 20),
                        String.Format("Roll:{0:f0}°", target.AngleZ));
                }
            }
        }

        public override void Activate()
        {
            if (IsActivate) return;
            SetFreeId();

                windowPosition.y = CurrentY;
                CurrentY = windowPosition.y+windowPosition.height;
              
            base.Activate();
        }

        public override void Deactivate()
        {
            if (!IsActivate) return;

                CurrentY = windowPosition.y;
                windowPosition.y = CurrentY - windowPosition.height;

            usedId.Remove(ID);
            base.Deactivate();
        }

        public void UpdateNoise() //whitenoise
        {
            idTextureNoise++;
            if (idTextureNoise >= 4)
                idTextureNoise = 0;
        }

        private void SetFreeId()
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!usedId.Contains(i))
                {
                    ID = i;
                    //lastVesselName = TargetHelper.Target.GetName();
                    //windowLabel = subWindowLabel + " " + ID + " to " + lastVesselName;
                    usedId.Add(i);
                    return;
                }
            }
        }

        public override void Update()
        {
            UpdateWhiteNoise();
            
            allCamerasGameObject.Last().transform.position = moduleDockingNodeGameObject.transform.position; // near&&far
            allCamerasGameObject.Last().transform.rotation = moduleDockingNodeGameObject.transform.rotation;
            
            allCamerasGameObject[0].transform.rotation = allCamerasGameObject.Last().transform.rotation; // skybox galaxy
            allCamerasGameObject[1].transform.rotation = allCamerasGameObject.Last().transform.rotation; // nature object
            allCamerasGameObject[2].transform.rotation = allCamerasGameObject.Last().transform.rotation; // middle 
            allCamerasGameObject[2].transform.position = allCamerasGameObject.Last().transform.position;
            allCameras.ForEach(cam => cam.fieldOfView = currentZoom);
        }
    }
}
