using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPCamera
{
    /// <summary>
    /// Module adds an external camera and gives control over it
    /// </summary>
    class CameraModule : PartModule, ICamPart
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Camera", isPersistant = true)]
        [UI_Toggle(controlEnabled = true, enabledText = "On", disabledText = "Off", scene = UI_Scene.All)]
        public bool IsEnabled;

        [KSPField]
        public string cameraName;
        
        [KSPField]
        public string rotatorZ ;

        [KSPField]
        public string rotatorY;

        [KSPField]
        public string cap;

        private GameObject capObject;

        [KSPField]
        public string zoommer;

        [KSPField]
        public float stepper;



        PartCamera camera;
        
        public override void OnStart(StartState state = StartState.Flying)
        {
            if (state == StartState.Editor || camera != null)
                return;
            Start();
        }
        public void Start()
        {
            if (camera == null)
                camera = new PartCamera(this.part, rotatorZ, rotatorY, zoommer, stepper, cap, cameraName);
            capObject = part.gameObject.GetChild(cap);
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

        public void Activate()
        {
            if (camera.IsActivate) return;
            camera.Activate();
            StartCoroutine("CapRotator");
        }
        public void Deavtivate()
        {
            if (!camera.IsActivate) return;
            camera.Deactivate();
            StartCoroutine("CapRotator");
            
        }
        private IEnumerator CapRotator()
        {
            int step = camera.IsActivate ? 5 : -5;
            for (int i = 0; i < 54; i++)
            {
                capObject.transform.Rotate(new Vector3(0, 1f, 0), step);
                yield return new WaitForSeconds(1f / 270);
            }
        }
        
    }
    interface ICamPart
    {
        /// <summary>
        /// Activate camera
        /// </summary>
        void Activate();
        /// <summary>
        /// Deactivate camera
        /// </summary>
        void Deavtivate();
        /// <summary>
        /// Adding a camera
        /// </summary>
        void Start();
    }
}
