using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPCamera
{
    class DockingCameraModule : PartModule, ICamPart
    {
        /// <summary>
        /// Module adds an external camera and gives control over it
        /// </summary>
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Camera", isPersistant = true)]
        [UI_Toggle(controlEnabled = true, enabledText = "On", disabledText = "Off", scene = UI_Scene.All)]
        public bool IsEnabled;

        [KSPField] public int allowedDistance;
        
        [KSPField]
        public bool noise = false;

        [KSPField] 
        public string nightVisionArgs = "0.5,0.7,0.5,0.5";
        private DockingCamera camera;

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
                camera = new DockingCamera(this.part, noise);
        }
        public override void OnUpdate()
        {
            if (camera == null) 
                return;
            if (camera.IsActivate)
                camera.Update();
            if (IsEnabled)
                Activate();
            else
                Deavtivate();
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
            if (TargetHelper.IsTargetSelect && new TargetHelper(part).Destination <= allowedDistance)
            {
                camera.Activate();
                StartCoroutine("WhiteNoiseUpdate"); //whitenoise
            }
            else
            {
                ScreenMessages.PostScreenMessage("You need to set target and be closer than " + allowedDistance + " meters from target", 5f, ScreenMessageStyle.UPPER_CENTER);
                IsEnabled = false;
            }
        }
        public void Deavtivate()
        {
            if (!camera.IsActivate) return;
            camera.Deactivate();
        }
    }
}
