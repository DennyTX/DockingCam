using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DockingCamera
{
    /// <summary>
    /// Module adds an external camera and gives control over it
    /// </summary>
    class PartCameraModule : PartModule, ICamPart
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Camera powered: ")]
        public string IsPowered;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Bullets: ")]
        public string aboutHits;

        [KSPField(isPersistant = true)]
        public int currentHits = -1;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Camera", isPersistant = true)]
        [UI_Toggle(controlEnabled = true, enabledText = "On", disabledText = "Off", scene = UI_Scene.All)]
        public bool IsEnabled;

        [KSPField]
        public int baseHits;

        [KSPField]
        public int windowSize = 256;

        [KSPField]
        public string cameraName;

        [KSPField]
        public string bulletName;

        [KSPField]
        public string rotatorZ ;

        [KSPField]
        public string rotatorY;

        [KSPField]
        public string cap;

        [KSPField]
        public string zoommer;

        [KSPField]
        public float stepper;

        [KSPField]
        public int distance;

        [KSPField]
        public string resource;

        [KSPField]
        public string resourceScanning;

        private GameObject capObject;

        private PartCamera camera;
        
        public override void OnStart(StartState state = StartState.Flying)
        {
            if (state == StartState.Editor || camera != null)
                return;
            Start();
        }
        public void Start()
        {
            if (camera == null)
            {
                //camera = new GameObject().AddComponent<PartCamera>();
                camera = new PartCamera(this.part, resourceScanning, bulletName, currentHits, rotatorZ, rotatorY, zoommer, stepper, cameraName, distance, windowSize);
            }
            capObject = part.gameObject.GetChild(cap);
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
            if (camera.IsToZero)
            {
                camera.IsToZero = false;
                StartCoroutine(camera.ReturnCamToZero());
            }
            if (camera.IsWaitForRay)
            {
                camera.IsWaitForRay = false;
                StartCoroutine(camera.WaitForRay());
            }
            currentHits = camera.hits;
            aboutHits = currentHits + "/4";
        }

        private void Update()
        {
            PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition("ElectricCharge");
            List<Part> parts = new List<Part>();
            parts = FlightGlobals.ActiveVessel.Parts;
            double ElectricChargeAmount = 0;
            foreach (Part p in parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (r.info.id == definition.id)
                    {
                        ElectricChargeAmount += r.amount;
                    }
                }
            }
            
            if (ElectricChargeAmount > 0)
            {
                if (IsEnabled)
                    IsPowered = "ACTIVE";
                else
                    IsPowered = "TRUE";                
            }
            else
                IsPowered = "FALSE";
        }

        public void Activate()
        {
            if (camera.IsActivate) return;
            camera.Activate();
            StartCoroutine("CapRotator");
        }
        public void Deactivate()
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
        void Deactivate();
        /// <summary>
        /// Adding a camera
        /// </summary>
        void Start();
    }
}
