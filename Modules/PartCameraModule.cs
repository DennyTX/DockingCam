using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        
        [KSPField]
        public string bulletName = "Sphere";

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Camera", isPersistant = true),
        UI_Toggle(controlEnabled = true, enabledText = "ON", disabledText = "OFF", scene = UI_Scene.All)]
        public bool IsEnabled;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "MODE:", isPersistant = true)]
        [UI_ChooseOption(options = new[] { "Onboard", "Look at Me", "Follow me", "Free Follow" })]
        public string GetOption = "Onboard";

        //[KSPField(guiActive = true, guiActiveEditor = false, guiName = "Look at Me", isPersistant = true)]
        //[UI_Toggle(controlEnabled = true, enabledText = "AIMED", disabledText = "OFF", scene = UI_Scene.Flight)]
        public bool IsLookAtMe;

        //[KSPField(guiActive = true, guiActiveEditor = false, guiName = "Follow", isPersistant = true)]
        //[UI_Toggle(controlEnabled = true, enabledText = "FOLLOW", disabledText = "OFF", scene = UI_Scene.Flight)]
        public bool IsFollow;
        public bool IsFollowEnabled;

        //[KSPField(guiActive = true, guiActiveEditor = false, guiName = "FreeFollow", isPersistant = true)]
        //[UI_Toggle(controlEnabled = true, enabledText = "ON DUTY", disabledText = "OFF", scene = UI_Scene.Flight)]
        public bool IsFreeFollow;
        public bool IsFreeFollowEnabled;

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Offset Distance X", isPersistant = true)]
        [UI_FloatRange(minValue = -50f, maxValue = 50f, stepIncrement = 5f)]
        public float IsFollowOffsetXXX;
        
        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Offset Distance Y", isPersistant = true)]
        [UI_FloatRange(minValue = -50f, maxValue = 50f, stepIncrement = 5f)]
        public float IsFollowOffsetYYY;

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Offset Distance Z", isPersistant = true)]
        [UI_FloatRange(minValue = -50f, maxValue = 50f, stepIncrement = 5f)]
        public float IsFollowOffsetZZZ;

        //[KSPField(guiActive = true, guiActiveEditor = false, guiName = "WorldPos", isPersistant = true)]
        //[UI_Toggle(controlEnabled = true, enabledText = "True", disabledText = "False", scene = UI_Scene.Flight)]
        //public bool WorldPos;

        [KSPField]
        public int windowSize = 256;

        [KSPField]
        public string cameraName = "CamExt";

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

        //[KSPField]
        //public string resource;

        [KSPField]
        public string resourceScanning;

        private GameObject capObject;
        private GameObject camObject;

        private PartCamera camera;

        private Vector3 initialLocalPosition;

        //private Transform CamToAim;
        
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
            camObject = part.gameObject.GetChild(cameraName);
            //camObject.transform.Rotate(new Vector3(0, 0, 1f), 90); 

            //var aaa1 = transform.FindChild("model");
            //Transform aaa11 = null;
            //foreach (Transform child in aaa1)
            //{
            //    //Debug.Log("AAAAAA "+child.name);
            //    if (child.name.IndexOf("OnboardCamera") != -1)
            //        aaa11 = child;
            //}
            //// var aaa11 = aaa1.FindChild("OLDD/DockingCam/OnboardCamera(Clone)");
            //var aaa2 = aaa11.FindChild("OnboardCamera_03");
            //var aaa3 = aaa2.FindChild("Case");
            //var aaa4 = aaa3.FindChild("Tube");
            //var aaa5 = aaa4.FindChild("Lenz");
            //CamToAim = aaa5.FindChild("CamExt");

        }

        public override void OnUpdate()
        {
            if (camera == null)
                return;
            var aaa = vessel.vesselModules;
            var bbb = FlightGlobals.ActiveVessel.vesselModules;
            if (camera.IsActivate)
                camera.Update();

            if (camera.IsButtonOff)
            {
                IsEnabled = false;
                camera.IsButtonOff = false;
            }

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

            GetElectricConsumption();
        }
        public void FixedUpdate()
        {
            if (IsEnabled)
            {
                Activate();
                switch (GetOption)
                {
                    case "Onboard":
                        SetCameraMode(false, false, false);
                        IsFollowOffsetXXX = IsFollowOffsetYYY = IsFollowOffsetZZZ = 0;
                        break;
                    case "Look at Me":
                        SetCameraMode(true, false, false);
                        IsFollowOffsetXXX = IsFollowOffsetYYY = IsFollowOffsetZZZ = 0;
                        break;
                    case "Follow me":
                        SetCameraMode(false, true, false);
                        break; 
                    case "Free Follow":
                        SetCameraMode(false, false, true);
                        break;
                }
                LookAtMe();
                Follow(); 
                FreeFollow();
            }
            else
            {
                SetCameraMode(false, false, false);
                Deactivate();
            }
        //    Fields["IsLookAtMe"].guiActive = IsEnabled;
        //    Fields["IsFollow"].guiActive = IsEnabled;
        //    Fields["IsFollowOffsetXXX"].guiActive = IsFollow;
        //    Fields["IsFollowOffsetYYY"].guiActive = IsFollow;
        //    Fields["IsFollowOffsetZZZ"].guiActive = IsFollow;
        //    Fields["WorldPos"].guiActive = IsFollow;
        }

        private void SetCameraMode(bool a, bool b, bool c)
        {
            IsLookAtMe = a;
            IsFollow = b;
            //IsFollowEnabled = c;
            IsFreeFollow = c;
            //IsFreeFollowEnabled = e;
        }

        private void LookAtMe()
        {
            if (IsLookAtMe)
            {
                //camera.IsAuxiliaryWindowOpen = false;  // block aux window
                //camera.IsAuxiliaryWindowButtonPres = true;
                   
                //IsFollow = false;
                //IsFollowEnabled = false;
                //IsFreeFollow = false;
                //IsFreeFollowEnabled = false;
                //IsFollowOffsetXXX = IsFollowOffsetYYY = IsFollowOffsetZZZ = 0;

                if (vessel != FlightGlobals.ActiveVessel)
                {
                    float dist = Vector3.Distance(vessel.transform.position, FlightGlobals.ActiveVessel.transform.position);
                    camera.currentZoom = dist > 100 ? 23 : camera.maxZoom;
                    camera.currentZoom = dist > 400 ? 17 : 23;
                    if (dist > 800)
                        camera.currentZoom = 9;
                    if (dist > 1600)
                        camera.currentZoom = 4;
                    camObject.transform.LookAt(FlightGlobals.ActiveVessel.transform, Vector3.forward);
                }
            }
            //else
            //{
            //    IsLookAtMe = false;
            //}
        }
        private void Follow()
        {
            if (IsFollow)
            {
                //IsLookAtMe = false;
                //IsFreeFollow = false;
                //IsFreeFollowEnabled = false;
                if (!IsFollowEnabled)
                {
                    camObject.transform.localPosition = new Vector3(IsFollowOffsetXXX, IsFollowOffsetYYY, IsFollowOffsetZZZ);
                } 
                if (vessel != FlightGlobals.ActiveVessel)
                {
                    if (!IsFollowEnabled)
                    {
                        camObject.transform.SetParent(FlightGlobals.ActiveVessel.transform, true);
                        IsFollowEnabled = true;
                    }
                }
            }
            else
            {
                //IsFollow = false;
                IsFollowEnabled = false;
            }
        }

        private void FreeFollow()
        {
            //var oldParent = camObject.transform.parent;
            //var initialLocalPosition = camObject.transform.localPosition;
            //float dist = Vector3.Distance(vessel.transform.position, FlightGlobals.ActiveVessel.transform.position);
            if (IsFreeFollow)
            {
                //IsLookAtMe = false;
                //IsFollow = false;
                //IsFollowEnabled = false;
                if (!IsFreeFollowEnabled)
                {
                    camObject.transform.localPosition = new Vector3(IsFollowOffsetXXX, IsFollowOffsetYYY, IsFollowOffsetZZZ);
                }
                if (vessel != FlightGlobals.ActiveVessel)
                {
                    if (!IsFreeFollowEnabled)
                    {
                        //camObject.transform.SetParent(FlightGlobals.ActiveVessel.transform, WorldPos);
                        initialLocalPosition = camObject.transform.position - FlightGlobals.ActiveVessel.transform.position;//camObject.transform.localPosition;
                        //initialRotation = camObject.transform;
                        IsFreeFollowEnabled = true;
                    }
                    camObject.transform.position = initialLocalPosition + FlightGlobals.ActiveVessel.transform.position;
                    camObject.transform.LookAt(FlightGlobals.ActiveVessel.transform, Vector3.forward);
                }
            }
            else
            {
                //IsFreeFollow = false;
                IsFreeFollowEnabled = false;
            }
        }

        private void GetElectricConsumption()
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
