using System.Collections;
using System.Linq;
using UnityEngine;

namespace DockingCamera
{
    class DockingCameraModule : PartModule, ICamPart
    {
        /// <summary>
        /// Module adds an external camera and gives control over it
        /// </summary>
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Camera", isPersistant = true)]
        [UI_Toggle(controlEnabled = true, enabledText = "On", disabledText = "Off", scene = UI_Scene.All)]
        
        public bool IsEnabled;

        //public bool IsTargeted;

        [KSPField]
        public int allowedDistance = 1000;

        [KSPField]
        public float maxSpeed = 3;

        [KSPField]
        public int windowSize = 256;
        
        [KSPField]
        public bool noise = false;

        [KSPField] 
        public string nightVisionArgs = "0.5,0.7,0.5,0.5";

        [KSPField]
        public string targetCrossColor = "0.9,0.0,0.0,1.0";

        [KSPField]
        public string targetCrossColorOLDD = "0.0,0.9,0.0,1.0";

        private new DockingCamera camera;

        public override void OnStart(PartModule.StartState state = StartState.Flying)
        {
            if (state == StartState.Editor || camera != null)
                return;
            Start();
        }

        public void Start()
        {
            CameraShaders.NightVisionArgs = nightVisionArgs;
            if (camera == null)
                camera = new DockingCamera(this.part, noise, windowSize);

            camera.MaxSpeed = maxSpeed;
            var splColorOLDD = targetCrossColorOLDD.Split(',').Select(float.Parse).ToList(); // parsing color to RGBA
            camera.TargetCrossColorOLDD = new Color(splColorOLDD[0], splColorOLDD[1], splColorOLDD[2], splColorOLDD[3]);
            var splColor = targetCrossColor.Split(',').Select(float.Parse).ToList(); // parsing color to RGBA
            camera.TargetCrossColor = new Color(splColor[0], splColor[1], splColor[2], splColor[3]);
        }

        public override void OnUpdate()
        {
            if (camera == null) 
                return;
            if (camera.IsActivate)
                camera.Update();
            if (camera.IsButtonOff)
            {
                IsEnabled = false;
                camera.IsButtonOff = false;
            }
            if (IsEnabled)
                Activate();
            else
                Deactivate();
            if (camera.IsAuxiliaryWindowButtonPres)
                StartCoroutine(camera.ResizeWindow());
        }

        private IEnumerator WhiteNoiseUpdate() //whitenoise
        {
            while (camera.IsActivate)
            {
                camera.UpdateNoise();
                yield return new WaitForSeconds(.1f);
            }
        }
        public void Activate()
        {
            if (camera.IsActivate) return;
            if (TargetHelper.IsTargetSelect )
            {
                var target = new TargetHelper(part);
                target.Update();
                if (target.Destination > allowedDistance)
                {
                    ScreenMessages.PostScreenMessage("You need to set target and be closer than " + allowedDistance + " meters from target", 5f, ScreenMessageStyle.UPPER_CENTER);
                    IsEnabled = false;
                }
                else
                {
                    camera.Activate();
                    //StartCoroutine(camera.ActivateOldTv(camera));
                    StartCoroutine("WhiteNoiseUpdate"); //whitenoise
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("You need to set target", 5f, ScreenMessageStyle.UPPER_CENTER);
                IsEnabled = false;
            }
        }
        public void Deactivate()
        {
            if (!camera.IsActivate) return;
            camera.Deactivate();
            //StartCoroutine(camera.DeactivateOldTv(camera));
        }

    }
}
