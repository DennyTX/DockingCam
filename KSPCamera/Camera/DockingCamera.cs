using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine; 


namespace KSPCamera
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
        private Texture2D texturePlaceTarget;
        private static List<Texture2D>[]textureWhiteNoise;
        private bool noiseActive;
        private int idTextureNoise;
        //private Rect positionWhiteNoise = new Rect(0,0,1024,1024);
        private static GUIStyle guiStyleRedLabel;
        private TargetHelper target;
        public DockingCamera(Part part, bool noiseActive, string windowLabel = "Docking Camera")
            : base(part, windowLabel)
        {
            this.noiseActive = noiseActive;
            target = new TargetHelper(part);
            guiStyleRedLabel = HighLogic.Skin.label;
            guiStyleRedLabel.normal.textColor = Color.red;
            if (textureWhiteNoise != null || !noiseActive) return;
            textureWhiteNoise = new List<Texture2D>[3];
            for (int j = 0; j < 3; j++)
            {
                textureWhiteNoise[j] = new List<Texture2D>();
                for (int i = 0; i < 4; i++)
                    textureWhiteNoise[j].Add(Util.WhiteNoiseTexture((int)texturePosition.width, (int)texturePosition.height));
                
            }
        }
        protected override void InitTextures()
        {
            base.InitTextures();
            //textureWhiteNoise = Util.LoadTexture("orig"); //whitenoise
            textureCentre = Util.LoadTexture("dockingcam");
            textureRotationSelf = Util.LoadTexture("selfrot");
            textureRotationTarget = Util.LoadTexture("targetrot");
            textureLampOn = Util.LoadTexture("lampon");
            textureLampOff = Util.LoadTexture("lampoff");
            textureVLine = Util.MonoColorVerticalLineTexture(new Color(0, .9f, 0, 1), (int)windowSize * windowSizeCoef);
            textureHLine = Util.MonoColorHorizontalLineTexture(new Color(0, .9f, 0, 1), (int)windowSize * windowSizeCoef);
        }
        protected override void ExtendedDrawWindowL2()
        {
            base.ExtendedDrawWindowL2();
            GUI.DrawTexture(texturePosition, textureCentre);
            if (noiseActive)
                GUI.DrawTexture(texturePosition, textureWhiteNoise[windowSizeCoef-2][idTextureNoise]);  //whitenoise
        }
        protected override void ExtendedDrawWindowL3()
        {

            if (GUI.RepeatButton(new Rect(7, 15, 20, 20), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }
            if (GUI.RepeatButton(new Rect(28, 15, 20, 20), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom;
            }
            if (TargetHelper.IsTargetSelect)
            {
                ///DATE block
                ///
                float i = 0;
                target.Update();
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("d:{0:f2}", target.Destination));
                i+=.25f;
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("dx:{0:f2}", target.DX));
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("dy:{0:f2}", target.DY));
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("dz:{0:f2}", target.DZ));
                i +=.25f;
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("°X:{0:f1}°", target.AngleX));
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("°Y:{0:f1}°", target.AngleY));
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("°Z:{0:f1}°", target.AngleZ));
                i += .25f;
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("vX:{0:f1}", target.SpeedX));
                GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("vY:{0:f1}", target.SpeedY));
                if (target.SpeedZ > 3)
                    GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("vZ:{0:f1}", target.SpeedZ), guiStyleRedLabel);
                else
                    GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("vZ:{0:f1}", target.SpeedZ));
                

                i += .25f;

                //LAMP&Seconds Block
                if (target.isMoveToTarget)
                {
                    GUI.Label(new Rect(texturePosition.xMax - 70, 20 + i++ * 20, 70, 20), String.Format("s:{0:f0}", target.SecondsToConnection));
                    GUI.DrawTexture(new Rect(texturePosition.xMin+20, texturePosition.yMax - 20, 20, 20), textureLampOn);
                }
                else
                    GUI.DrawTexture(new Rect(texturePosition.xMin + 20, texturePosition.yMax - 20, 20, 20), textureLampOff);
                //RotationXY Block
                
                var tx = texturePosition.width/2;
                var ty = texturePosition.height / 2;
                if (Mathf.Abs(target.AngleX) > 20)
                    tx += (target.AngleX > 0 ? -1 : 1) * (texturePosition.width / 2 - 1);
                else
                    tx +=  (texturePosition.width / 40) * -target.AngleX;
                if (Mathf.Abs(target.AngleY) > 20)
                    ty += (target.AngleY > 0 ? -1 : 1) * (texturePosition.height / 2 - 1);
                else
                    ty += (texturePosition.height / 40) * -target.AngleY;

                GUI.DrawTexture(new Rect(texturePosition.xMin + tx, texturePosition.yMin, 1, texturePosition.height), textureVLine);
                GUI.DrawTexture(new Rect(texturePosition.xMin, texturePosition.yMin + ty, texturePosition.width, 1), textureHLine);
                //Place Block

                //tx = texturePosition.width / 2;
                //ty = texturePosition.height / 2;
                //tx += (texturePosition.width / 40) * -target.AngleX;
                //if (Mathf.Abs(target.AngleY) > 20)
                //    ty += (target.AngleY > 0 ? -1 : 1) * (texturePosition.height / 2 - 1);
                //else
                //    ty += (texturePosition.height / 40) * -target.AngleY;


                //RotationZ Block
                var size = texturePosition.width / 5;
                var x = texturePosition.xMin+texturePosition.width/2-size/2-22;
                GUI.DrawTexture(new Rect(x, texturePosition.yMax - size, size, size), textureRotationSelf);
                Matrix4x4 matrixBackup = GUI.matrix;
                var position = new Rect(x, texturePosition.yMax - size, size, size);
                GUIUtility.RotateAroundPivot(-target.AngleZ, position.center);//new Vector2(x + size / 2, texturePosition.yMax - size / 2));
                GUI.DrawTexture(position, textureRotationTarget);
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
                    windowLabel = subWindowLabel + " " + ID + " " + TargetHelper.Target.GetName();
                    usedId.Add(i);
                    return;
                }
            }
        }
    }
}
