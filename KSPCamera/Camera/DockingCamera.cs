using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 


namespace DockingCamera
{
    class DockingCamera:BaseKspCamera
    {
        private HashSet<int> usedId = new HashSet<int>();
        private int ID;
        private Texture2D textureCentre;
        private Texture2D textureVLine;
        private Texture2D textureHLine;
        private Texture2D textureRotationSelf;
        private Texture2D textureRotationTarget;
        private Texture2D textureLampOn;
        private Texture2D textureLampOff;
        //private Texture2D texturePlaceTarget;
        private static List<Texture2D>[]textureWhiteNoise;
        private bool noiseActive;
        private int idTextureNoise;
        private static GUIStyle guiStyleRedLabel;
        private static GUIStyle guiStyleGreenLabel;
        private TargetHelper target;
        private string lastVesselName;
        private bool cameraData = true;
        private bool rotatorState = true;
        private bool targetCrossState = true;
        private GameObject moduleDockingNodeGameObject;
        public float MaxSpeed = 3;
        private Color targetCrossColor = new Color(0, .9f, 0, 1);

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


        public DockingCamera(Part part, bool noiseActive, string windowLabel = "DockCam")
            : base(part, windowLabel)
        {
            this.noiseActive = noiseActive;
            target = new TargetHelper(part);
            guiStyleRedLabel = new GUIStyle(HighLogic.Skin.label);
            guiStyleGreenLabel = new GUIStyle(HighLogic.Skin.label);
            guiStyleRedLabel.normal.textColor = Color.red;
            guiStyleGreenLabel.normal.textColor = Color.green;

            GameEvents.onVesselChange.Add(vesselChange);

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

        ~DockingCamera()  //деструктор
        {
            GameEvents.onVesselChange.Remove(vesselChange);
        }

        private void vesselChange(Vessel vessel)
        {
            windowLabel = subWindowLabel + " " + ID + " to " + lastVesselName;
        }

        protected override void InitTextures()
        {
            base.InitTextures();
            textureCentre = Util.LoadTexture("dockingcam");
            textureRotationSelf = Util.LoadTexture("selfrot");
            textureRotationTarget = Util.LoadTexture("targetrot");
            textureLampOn = Util.LoadTexture("lampon");
            textureLampOff = Util.LoadTexture("lampoff");
            textureVLine = Util.MonoColorVerticalLineTexture(TargetCrossColor, (int)windowSize * windowSizeCoef);
            textureHLine = Util.MonoColorHorizontalLineTexture(TargetCrossColor, (int)windowSize * windowSizeCoef);
        }

        protected override void ExtendedDrawWindowL1()
        {
            cameraData = GUI.Toggle(new Rect(windowPosition.width - 78, 34, 76, 36), cameraData, "Flight\n data");
            rotatorState = GUI.Toggle(new Rect(windowPosition.width - 78, 72, 76, 20), rotatorState, "Rotator");
            targetCrossState = GUI.Toggle(new Rect(windowPosition.width - 78, 92, 76, 20), targetCrossState, "Cross");
            base.ExtendedDrawWindowL1();
        }

        protected override void ExtendedDrawWindowL2()
        {
            GUI.DrawTexture(texturePosition, textureCentre);
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
                    textureLampOn);
                GUI.Label(new Rect(texturePosition.xMin + 40, texturePosition.yMax - 20, 140, 20),
                    String.Format("Time to dock:{0:f0}s", target.SecondsToDock));
            }
            else
                GUI.DrawTexture(new Rect(texturePosition.xMin + 20, texturePosition.yMax - 20, 20, 20),
                    textureLampOff);

            if (cameraData)
            {
                if (TargetHelper.IsTargetSelect && part.vessel.Equals(FlightGlobals.ActiveVessel))
                {
                    /// <summary>
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
                                "DockPort is\n available",guiStyleGreenLabel);
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
                    if (target.SpeedZ > MaxSpeed)
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
                    //i += .25f;
                }
            }

            if (targetCrossState)
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
                    textureVLine);
                GUI.DrawTexture(
                    new Rect(texturePosition.xMin, texturePosition.yMin + ty, texturePosition.width, 1),
                    textureHLine);

                //RotationXY Block SE START
                //var tx = texturePosition.width / 2;
                //var ty = texturePosition.height / 2;
                //if (Mathf.Abs(target.AngleX) > 20)
                //    tx += (target.AngleX > 0 ? -1 : 1) * (texturePosition.width / 2 - 1);
                //else
                //    tx += (texturePosition.width / 40) * -target.AngleX;
                //if (Mathf.Abs(target.AngleY) > 20)
                //    ty += (target.AngleY > 0 ? -1 : 1) * (texturePosition.height / 2 - 1);
                //else
                //    ty += (texturePosition.height / 40) * -target.AngleY;
                //Matrix4x4 _matrixBackup = GUI.matrix;
                //GUIUtility.RotateAroundPivot(-target.AngleZ, new Vector2(tx,ty));
                //GUI.DrawTexture(new Rect(texturePosition.xMin + tx, -250, 1, 1000),textureVLine);
                //GUI.DrawTexture(new Rect(-250, texturePosition.yMin + ty, 1000, 1),textureHLine);
                //GUI.matrix = _matrixBackup;
                //RotationXY Block SE END
            }

            //RotationZ Block
            //if (rotatorState) // && TargetHelper.IsTargetSelect && part.vessel.Equals(FlightGlobals.ActiveVessel))
            //{
            //    var size = texturePosition.width / 5;
            //    var x = texturePosition.xMin + texturePosition.width / 2 - size / 2;
            //    GUI.DrawTexture(new Rect(x, texturePosition.yMax - size, size, size), textureRotationSelf);
            //    Matrix4x4 matrixBackup = GUI.matrix;
            //    var position = new Rect(x, texturePosition.yMax - size, size, size);
            //    GUIUtility.RotateAroundPivot(-target.AngleZ, position.center);
            //    //new Vector2(x + size / 2, texturePosition.yMax - size / 2));
            //    GUI.DrawTexture(position, textureRotationTarget);
            //    GUI.matrix = matrixBackup;
            //}
            if (rotatorState) // && TargetHelper.IsTargetSelect && part.vessel.Equals(FlightGlobals.ActiveVessel))
            {
                var size = texturePosition.width / 8  ;
                var x = texturePosition.xMin + texturePosition.width / 2 - size / 2;
                GUI.DrawTexture(new Rect(x, texturePosition.yMax - size, size, size), textureRotationTarget);
                Matrix4x4 matrixBackup = GUI.matrix;
                var position = new Rect(x, texturePosition.yMax - size, size, size);
                GUIUtility.RotateAroundPivot(target.AngleZ, position.center);
                //new Vector2(x + size / 2, texturePosition.yMax - size / 2));
                GUI.DrawTexture(position, textureRotationSelf);
                GUI.matrix = matrixBackup;
            }

            base.ExtendedDrawWindowL3();
        }

        public override void Activate()
        {
            if (IsActivate) return;
            SetFreeId();
            base.Activate();
        }

        public override void Deactivate()
        {
            if (!IsActivate) return;
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
                    windowLabel = subWindowLabel + " " + ID + " to " + TargetHelper.Target.GetName();
                    lastVesselName = TargetHelper.Target.GetName();
                    usedId.Add(i);
                    return;
                }
            }
        }
        public override void Update()
        {
            var camers = Camera.allCameras;
            allCamerasGameObject.Last().transform.position = moduleDockingNodeGameObject.transform.position; // near&&far
            allCamerasGameObject.Last().transform.rotation = moduleDockingNodeGameObject.transform.rotation;
            //allCamerasGameObject.Last().transform.Rotate(new Vector3(1f, 0, 0), 90);
            
            allCamerasGameObject[0].transform.rotation = allCamerasGameObject.Last().transform.rotation; // skybox galaxy
            allCamerasGameObject[1].transform.rotation = allCamerasGameObject.Last().transform.rotation; // nature object
            allCamerasGameObject[2].transform.rotation = allCamerasGameObject.Last().transform.rotation; // middle 
            allCamerasGameObject[2].transform.position = allCamerasGameObject.Last().transform.position;
            allCameras.ForEach(cam => cam.fieldOfView = currentZoom);
        }
    }
}
