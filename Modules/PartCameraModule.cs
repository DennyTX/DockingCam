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
        public Transform lookAtTarget;
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
        //}

        //private void Update()
        //{
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

            var aaa = vessel.name;
            var bbb = FlightGlobals.ActiveVessel.name;
            if (aaa != bbb)
            {
            //    var CamToAim = GameObject.Find("CamExt"); // это поиск не в конкретном месте, а во всей сцене.

            //if (lookAtTarget != null)
            //{
                var aaa1 = transform.FindChild("model");
                Transform aaa11 = null;
                foreach (Transform child in aaa1)
                {
                    //Debug.Log("AAAAAA "+child.name);
                    if (child.name.IndexOf("OnboardCamera") != -1)
                        aaa11 = child;
                }
               // var aaa11 = aaa1.FindChild("OLDD/DockingCam/OnboardCamera(Clone)");
                var aaa2 = aaa11.FindChild("OnboardCamera_03");
                var aaa3 = aaa2.FindChild("Case");
                var aaa4 = aaa3.FindChild("Tube");
                var aaa5 = aaa4.FindChild("Lenz");
                var CamToAim = aaa5.FindChild("CamExt");

                CamToAim.transform.LookAt(FlightGlobals.ActiveVessel.transform.position);

                //CamToAim.transform.LookAt(lookAtTarget.position);
            }
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
