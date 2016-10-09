using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSP.IO;
using UnityEngine;

namespace OLDD_camera.Camera
{
    class PartCamera : BaseKspCamera
    {
        private static HashSet<int> usedId = new HashSet<int>();

        private PluginConfiguration config;

        private static float CurrentX = -32;
        private static float CurrentY = 32;
        private int buttonSize = 25;

        private GameObject rotatorZ;
        private GameObject rotatorY;
        private GameObject zoommer;
        private GameObject camObject;
        private LineRenderer scanningRay;
        private LineRenderer visibilityRay;

        private float stepper;
        internal float rotateZBuffer;
        internal float rotateYBuffer;
        private float zoomBuffer;
        private float lastZoom;
        private float simplifiedRotateZBuffer;
        private float rotateStep;
        private int ID;
        private int AllowedDistance;
        public int hits = 4;
        private int ResourceUsage;
        private string ResourceName;
        private string bulletName;
        private bool IsRayEnabled;
        private bool IsUpsideDown;
        private bool IsScienceActivate;
        private bool IsVisibilityRay;
        public bool IsWaitForRay;
        public bool IsToZero;

        internal bool IsOnboard;
        internal bool IsOnboardEnabled;
        internal bool IsLookAtMe;
        internal bool IsLookAtMeEnabled;
        internal bool IsLookAtMeAutoZoom;
        internal bool IsFollowMe;
        internal bool IsFollowEnabled;
        internal bool IsTargetCam;
        internal bool IsTargetCamEnabled;
        internal bool IsOutOfRange;

        internal string cameraMode;

        internal float IsFollowMeOffsetXXX;
        internal float IsFollowMeOffsetYYY;
        internal float IsFollowMeOffsetZZZ;
        //private float _targetOffset = 100;
        internal Transform CurrentCamTarget;
        internal Transform CurrentCam;

        internal Quaternion _initialCamRotation;
        internal Vector3 _initialCamPosition;
        internal Quaternion _initialCamLocalRotation;
        internal Vector3 _initialCamLocalPosition;

        internal Quaternion _currentCamRotation;
        internal Vector3 _currentCamPosition;
        internal Quaternion _currentCamLocalRotation;
        internal Vector3 _currentCamLocalPosition;

        public float realZoom
        {
            get { return (zoomMultiplier ? currentZoom / minZoomMultiplier : currentZoom); }
            set { currentZoom = value; }
        }

        public PartCamera(Part part, string resourceScanning, string bulletName, int _hits,
                string rotatorZ, string rotatorY, string zoommer, float stepper, string cameraName, int allowedDistance, int windowSize,
                bool _IsOnboard, bool _IsLookAtMe, bool _IsLookAtMeAutoZoom, bool _IsFollowMe, bool _IsTargetCam,
                float _IsFollowMeOffsetXXX, float _IsFollowMeOffsetYYY, float _IsFollowMeOffsetZZZ,
                string windowLabel = "Camera")
            : base(part, windowSize, windowLabel)
        {
            var splresource = resourceScanning.Split('.').ToList();
            ResourceName = splresource[0];
            ResourceUsage = Int32.Parse(splresource[1]);
            this.bulletName = bulletName;
            this.rotatorZ = partGameObject.gameObject.GetChild(rotatorZ);
            this.rotatorY = partGameObject.gameObject.GetChild(rotatorY);
            this.zoommer = partGameObject.gameObject.GetChild(zoommer);
            this.stepper = stepper;
            camObject = partGameObject.gameObject.GetChild(cameraName);
            AllowedDistance = allowedDistance;

            IsOnboard = _IsOnboard;
            IsLookAtMe = _IsLookAtMe;
            IsFollowMe = _IsFollowMe;
            IsLookAtMeAutoZoom = _IsLookAtMeAutoZoom;
            IsTargetCam = _IsTargetCam;
            IsFollowMeOffsetXXX = _IsFollowMeOffsetXXX;
            IsFollowMeOffsetYYY = _IsFollowMeOffsetYYY;
            IsFollowMeOffsetZZZ = _IsFollowMeOffsetZZZ;

            lastZoom = currentZoom;

            GameEvents.onGameSceneLoadRequested.Add(LevelWasLoaded);

            GetCurrentBullets(bulletName, _hits);
        }

        private void GetCurrentBullets(string bulletName, int _hits)
        {
            if (_hits == -1)
            {
                hits = 0;
                while (true)
                {
                    var hit = partGameObject.GetChild(string.Format("{0}{1:000}", bulletName, hits + 1));
                    if (hit == null)
                        break;
                    hits++;
                }
            }
            else
            {
                hits = _hits;
                var i = hits + 1;
                while (true)
                {
                    var hit = partGameObject.GetChild(string.Format("{0}{1:000}", bulletName, i));
                    if (hit == null)
                        break;
                    GameObject.Destroy(hit);
                    i++;
                }
            }
        }

        public override void Activate()
        {
            base.Activate();
            SetFreeId();
            windowPosition.x = CurrentX + 32;
            windowPosition.y = CurrentY + 32;
            CurrentX = windowPosition.x;
            CurrentY = windowPosition.y;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            windowPosition.x = -32 + 32 * ID;
            windowPosition.y = 32 + 32 * ID;
            windowPosition.x -= 32;
            windowPosition.y -= 32;
            CurrentX = windowPosition.x;
            CurrentY = windowPosition.y;
            usedId.Remove(ID);
        }

        private void SetFreeId()
        {
            for (int i = 1; i < 8; i++)
            {
                if (!usedId.Contains(i))
                {
                    ID = i;
                    windowLabel = subWindowLabel + " " + ID;
                    usedId.Add(i);
                    return;
                }
            }
        }

        private void LevelWasLoaded(GameScenes data)
        {
            usedId = new HashSet<int>();
            CurrentX = -32;
            CurrentY = 32;
        }

        ~PartCamera()
        {
            GameEvents.onGameSceneLoadRequested.Remove(LevelWasLoaded);
        }

        protected override void ExtendedDrawWindowL1()
        {
            if (IsOrbital) return;

            SetRotationStep();

            var widthOffset = windowPosition.width - 90;

            if (IsOnboard)
                //{
                //    if (DrawButtonsBlock(widthOffset)) return;                
                //}
                DrawButtonsBlock(widthOffset);
            else
                DrawModeDataBlock(widthOffset);

            DrawModeSelector(widthOffset);

            zoomMultiplier = GUI.Toggle(new Rect(widthOffset, 112, 80, 20), zoomMultiplier, " x 24");

            if (IsOnboard)
            {
                GUI.Label(new Rect(widthOffset, 148, 80, 20), string.Format("rotateZ: {0:F0}°", simplifiedRotateZBuffer));
                GUI.Label(new Rect(widthOffset, 164, 80, 20), string.Format("rotateY: {0:F0}°", rotateYBuffer));
            }

            if (GUI.Button(new Rect(widthOffset, 186, 80, 25), "PHOTO"))
                renderTexture.SavePng(part.vessel.vesselName);

            if (IsOnboard || IsLookAtMe)
                IsVisibilityRay = GUI.Toggle(new Rect(widthOffset - 2, 215, 80, 20), IsVisibilityRay, "Target Ray");

            //isTargetPoint = GUI.Toggle(new Rect(widthOffset - 2, 233, 88, 20), isTargetPoint, "Target Mark");

            GUI.Label(new Rect(widthOffset, 311, 80, 20), string.Format("Bullets: {0:F0}", hits), guiStyleLabelBold);

            base.ExtendedDrawWindowL1();
        }

        private void DrawModeDataBlock(float widthOffset)
        {
            if (IsLookAtMe)
            {
                GUI.Box(new Rect(widthOffset - 4, 36, 86, 74), "Look At Me");
                GUI.Label(new Rect(widthOffset, 52, 84, 44), "Focus on:\n" + FlightGlobals.ActiveVessel.vesselName);
                IsLookAtMeAutoZoom = GUI.Toggle(new Rect(widthOffset, 86, 86, 20), IsLookAtMeAutoZoom, "AutoZoom");
            }

            if (IsFollowMe)
            {
                GUI.Box(new Rect(widthOffset, 36, 86, 74), "Offset X,Y,Z");
                IsFollowMeOffsetXXX = GUI.HorizontalSlider(new Rect(widthOffset + 1, 58, 80, 10), IsFollowMeOffsetXXX, -80, 80);
                IsFollowMeOffsetYYY = GUI.HorizontalSlider(new Rect(widthOffset + 1, 74, 80, 10), IsFollowMeOffsetYYY, -80, 80);
                IsFollowMeOffsetZZZ = GUI.HorizontalSlider(new Rect(widthOffset + 1, 90, 80, 10), IsFollowMeOffsetZZZ, -80, 80);
            }

            if (IsTargetCam)
            {
                GUI.Box(new Rect(widthOffset - 2, 36, 86, 74), "Target Data");
                var target = TargetHelper.Target as Vessel;
                if (target != null)
                    GUI.Label(new Rect(widthOffset + 4, 52, 84, 44), "Focus on:\n" + TargetHelper.Target.GetName());
                else
                    GUI.Label(new Rect(widthOffset + 4, 56, 84, 22), "NO TARGET");
            }
        }

        protected override void ExtendedDrawWindowL2()
        {
            if (IsTargetCam && IsOutOfRange)
            {
                Graphics.DrawTexture(texturePosition, textureNoSignal[textureNoSignalId]);
                GUI.Label(new Rect(texturePosition.xMin + 32 * windowSizeCoef, texturePosition.yMin + 32 * windowSizeCoef, 160, 160),
                "TARGET \n IS \n OUT OF RANGE", guiStyleRedLabelBoldLarge);
            }
            base.ExtendedDrawWindowL2();
        }

        protected override void ExtendedDrawWindowL3()
        {
            var str = "Mode: " + cameraMode + " ( x " + calculatedZoom + " )";
            GUI.Label(new Rect(texturePosition.xMin + 44 * windowSizeCoef, texturePosition.yMax - 16, 160, 20),
                str, guiStyleGreenLabelSmall);
            base.ExtendedDrawWindowL3();
        }

        private void DrawModeSelector(float widthOffset)
        {
            GUI.Box(new Rect(widthOffset - 2, 256, 86, 56), "");

            if (part.vessel != FlightGlobals.ActiveVessel)
            {
                if (IsLookAtMe = GUI.Toggle(new Rect(widthOffset - 2, 256, 84, 20), IsLookAtMe, "Look at Me"))
                {
                    IsOnboard = IsFollowMe = IsTargetCam = false;
                    var lastCameraMode = cameraMode;
                    cameraMode = "Look at Me";
                    IsFollowMeOffsetXXX = IsFollowMeOffsetYYY = IsFollowMeOffsetZZZ = 0;
                    if (!IsLookAtMeEnabled)
                    {
                        CameraPositioning(lastCameraMode);
                    }
                }
            }

            if (IsFollowMe = GUI.Toggle(new Rect(widthOffset - 2, 274, 84, 20), IsFollowMe, "Follow Me"))
            {
                IsOnboard = IsLookAtMe = IsTargetCam = false;
                var lastCameraMode = cameraMode;
                cameraMode = "Follow Me";
                if (!IsFollowEnabled)
                {
                    CameraPositioning(lastCameraMode);
                }
            }

            //if (part.vessel == FlightGlobals.ActiveVessel)
            //{
            if (IsTargetCam = GUI.Toggle(new Rect(widthOffset - 2, 292, 84, 20), IsTargetCam, "Target Cam"))
            {
                IsOnboard = IsLookAtMe = IsFollowMe = false;
                var lastCameraMode = cameraMode;
                cameraMode = "Target Cam";
                if (!IsTargetCamEnabled)
                {
                    CameraPositioning(lastCameraMode);
                    currentZoom = 32;
                    zoomMultiplier = false;
                }
                var target = TargetHelper.Target as Vessel;
                if (target != null)
                {
                    var range = Mathf.Round(Vector3.Distance(target.transform.position, FlightGlobals.ActiveVessel.transform.position));
                    if (range > FlightGlobals.ActiveVessel.vesselRanges.landed.load)
                        IsOutOfRange = true;
                    else
                        IsOutOfRange = false;
                }
            }
            //}
            else if (!IsLookAtMe && !IsFollowMe && !IsTargetCam)
            {
                IsOnboard = true;
                //IsLookAtMe = IsFollowMe = IsTargetCam = false;
                cameraMode = "Onboard";
                IsFollowMeOffsetXXX = IsFollowMeOffsetYYY = IsFollowMeOffsetZZZ = 0;
                if (!IsOnboardEnabled)
                {
                    camObject.transform.rotation = _currentCamRotation;
                    camObject.transform.position = _currentCamPosition;
                    camObject.transform.localRotation = _currentCamLocalRotation;
                    camObject.transform.localPosition = _currentCamLocalPosition;
                    currentZoom = 32;
                    zoomMultiplier = false;
                    IsOnboardEnabled = true;
                }
            }
        }

        private void CameraPositioning(string lastCameraMode)
        {
            if (lastCameraMode == "Onboard")
            {
                _currentCamRotation = camObject.transform.rotation;
                _currentCamPosition = camObject.transform.position;
                _currentCamLocalRotation = camObject.transform.localRotation;
                _currentCamLocalPosition = camObject.transform.localPosition;
            }
            camObject.transform.rotation = _initialCamRotation;
            camObject.transform.position = _initialCamPosition;
            camObject.transform.localRotation = _initialCamLocalRotation;
            camObject.transform.localPosition = _initialCamLocalPosition;
        }

        //private bool DrawButtonsBlock(float widthOffset)
        private void DrawButtonsBlock(float widthOffset)
        {
            if (GUI.Button(new Rect(widthOffset, 36, buttonSize, buttonSize), "↻"))
            {
                camObject.transform.Rotate(new Vector3(0, 0, 180f));
                IsUpsideDown = !IsUpsideDown;
            }
            if (GUI.RepeatButton(new Rect(widthOffset + buttonSize, 36, buttonSize, buttonSize), "↑"))
            {
                if (rotateYBuffer < 180)
                {
                    if (!IsUpsideDown)
                        rotateY += rotateStep;
                    else
                        rotateY -= rotateStep;
                }
            }
            if (GUI.Button(new Rect(widthOffset + buttonSize * 2, 36, buttonSize, buttonSize), "⦿"))
            {
                //isTargetVisiable();
                if (hits <= 0)
                {
                    ScreenMessages.PostScreenMessage("BULLETS DEPLETED", 3f, ScreenMessageStyle.UPPER_CENTER);
                    //return true;
                }
                if (!TargetHelper.IsTargetSelect)
                {
                    ScreenMessages.PostScreenMessage("NO TARGET FOR SCANNING", 3f, ScreenMessageStyle.UPPER_CENTER);
                    //return true;
                }
                if (HitCounter() && UseResourceForScanning())
                {
                    IsRayEnabled = true;
                    IsWaitForRay = true;
                    IsScienceActivate = false;
                }
            }
            if (GUI.RepeatButton(new Rect(widthOffset, 36 + buttonSize, buttonSize, buttonSize), "←"))
            {
                if (!IsUpsideDown)
                    rotateZ -= rotateStep;
                else
                    rotateZ += rotateStep;
            }
            if (GUI.Button(new Rect(widthOffset + buttonSize, 36 + buttonSize, buttonSize, buttonSize), "o"))
            {
                IsToZero = true;
            }
            if (GUI.RepeatButton(new Rect(widthOffset + buttonSize * 2, 36 + buttonSize, buttonSize, buttonSize), "→"))
            {
                if (!IsUpsideDown)
                    rotateZ += rotateStep;
                else
                    rotateZ -= rotateStep;
            }
            if (GUI.Button(new Rect(widthOffset, 36 + buttonSize * 2, buttonSize, buttonSize), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom;
            }
            if (GUI.RepeatButton(new Rect(widthOffset + buttonSize, 36 + buttonSize * 2, buttonSize, buttonSize), "↓"))
            {
                if (rotateYBuffer > 0)
                    if (!IsUpsideDown)
                        rotateY -= rotateStep;
                    else
                        rotateY += rotateStep;
            }
            if (GUI.Button(new Rect(widthOffset + buttonSize * 2, 36 + buttonSize * 2, buttonSize, buttonSize), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }
            //return false;
        }

        private void SetRotationStep()
        {
            simplifiedRotateZBuffer = rotateZBuffer;
            if (Mathf.Abs(simplifiedRotateZBuffer) > 360)
            {
                simplifiedRotateZBuffer = simplifiedRotateZBuffer % 360;
            }

            if ((maxZoom - currentZoom + minZoom) <= 10)
            {
                rotateStep = 0.8f;
            }
            if ((maxZoom - currentZoom + minZoom) <= 20)
            {
                rotateStep = 0.4f;
            }
            if ((maxZoom - currentZoom + minZoom) <= 32)
            {
                rotateStep = 0.2f;
            }
            if (zoomMultiplier)
            {
                rotateStep = 0.02f;
            }
        }

        public override void Update()
        {
            if (IsOrbital || IsOutOfRange)
                UpdateWhiteNoise();
            DrawScanningRay();
            DrawVisibilityRay();

            allCamerasGameObject.Last().transform.position = camObject.gameObject.transform.position;
            allCamerasGameObject.Last().transform.rotation = camObject.gameObject.transform.rotation;

            var step = -(lastZoom - currentZoom) / stepper;
            lastZoom = currentZoom;
            zoommer.transform.Translate(new Vector3(step, 0, 0));
            rotatorZ.transform.Rotate(new Vector3(0, 0, 1), rotateZ);
            rotatorY.transform.Rotate(new Vector3(0, 1, 0), rotateY);
            rotateZBuffer += rotateZ;
            rotateYBuffer += rotateY;
            zoomBuffer += step;

            allCamerasGameObject[0].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[1].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[2].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[2].transform.position = allCamerasGameObject.Last().transform.position;
            allCameras.ForEach(cam => cam.fieldOfView = realZoom); //currentZoom); 
            rotateZ = 0;
            rotateY = 0;
        }

        void DrawScanningRay()
        {
            GameObject.Destroy(scanningRay);
            if (IsRayEnabled && TargetHelper.IsTargetSelect)
            {
                Vector3 endPoint;
                if (isInsight(out endPoint))
                {
                    //scanningRay = new LineRenderer();
                    //create a new empty gameobject and scanningRay renderer component
                    scanningRay = new GameObject("scanningRay").AddComponent<LineRenderer>();
                    //assign the material to the scanningRay
                    scanningRay.SetColors(Color.red, Color.red);
                    //set the number of points to the scanningRay
                    scanningRay.SetVertexCount(2);
                    scanningRay.SetWidth(0.02f, 0.02f);
                    //render scanningRay to the world origin and not to the object's position
                    scanningRay.useWorldSpace = true;
                    scanningRay.SetPosition(0, part.transform.position);
                    scanningRay.SetPosition(1, endPoint);
                }
            }
        }

        private void DrawVisibilityRay()
        {
            GameObject.Destroy(visibilityRay);
            if (IsVisibilityRay)
            {
                if (!TargetHelper.IsTargetSelect || !isTargetVisiable()) return;
                //create a new empty gameobject and scanningRay renderer component
                visibilityRay = new GameObject("visibilityRay").AddComponent<LineRenderer>();
                var color = Color.white;
                visibilityRay.SetColors(color, color);
                //set the number of points to the scanningRay
                visibilityRay.SetVertexCount(2);
                visibilityRay.SetWidth(0.02f, 0.04f);
                //render visibilityRay
                visibilityRay.useWorldSpace = true;
                visibilityRay.SetPosition(0, camObject.transform.position);
                visibilityRay.SetPosition(1, TargetHelper.Target.GetTransform().position);
            }
        }

        private bool UseResourceForScanning()
        {
            //var res = part.vessel.GetActiveResources().First(x => x.info.name == ResourceName);
            //if (res == null)
            //    return false;
            //if (res.amount < ResourceUsage)
            //{
            //    part.RequestResource(ResourceName, ResourceUsage);
            //    return false;
            //}
            //part.RequestResource(ResourceName, ResourceUsage);
            return true;
        }

        private bool HitCounter()
        {
            var hit = partGameObject.GetChild(string.Format("{0}{1:000}", bulletName, hits));
            GameObject.Destroy(hit);
            hits--;
            return true;
        }

        private bool isInsight(out Vector3 endPoint)
        {
            var camera = allCameras.Last();
            endPoint = TargetHelper.Target.GetTransform().position;
            var point = camera.WorldToViewportPoint(endPoint); //get current targeted vessel 
            var x = point.x; // (0;1)
            var y = point.y; // (0;1)
            var z = point.z;
            return (z > 0 && 0 <= x && x <= 1 && 0 <= y && y <= 1);
        }

        private bool isTargetVisiable()
        {

            foreach (var body in FlightGlobals.Bodies)
            {
                var r = body.Radius;
                var self = part.vessel.GetWorldPos3D();
                var target = TargetHelper.Target.GetTransform().position;
                var shift = target - self;
                var coef = r / Vector3.Distance(self, target);
                coef *= .5f;
                shift *= coef;
                var point = target - shift;
                var distanceFromPoint = Vector3.Distance(body.position, point);
                if (distanceFromPoint < r)
                    return false;
            }
            return true;
        }

        public IEnumerator WaitForRay()
        {
            yield return new WaitForSeconds(1);
            IsRayEnabled = false;
            var target = new TargetHelper(part);
            target.Update();
            Vector3 endPoint;
            if (target.Destination <= AllowedDistance && isInsight(out endPoint) && isTargetVisiable())
            {
                ScreenMessages.PostScreenMessage(FlightGlobals.activeTarget.vessel.vesselName + " HAS BEEN SCANNED", 3f, ScreenMessageStyle.UPPER_CENTER);
                if (!IsScienceActivate)
                {
                    var spyScience = part.GetComponent<ModuleSpyExperiment>();
                    if (spyScience != null)
                        spyScience.DeployExperiment();
                    IsScienceActivate = true;
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("NO DATA, TARGET " + FlightGlobals.activeTarget.vessel.vesselName + " IS OUT OF RANGE  OR VISIBILITY", 3f, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        public IEnumerator ReturnCamToZero()
        {
            var coef = 20;
            var stepRotZ = -simplifiedRotateZBuffer / coef;
            var stepRotY = -rotateYBuffer / coef;
            var stepZoom = -zoomBuffer / coef;
            for (int i = 0; i < coef; i++)
            {
                zoommer.transform.Translate(new Vector3(stepZoom, 0, 0));
                rotatorZ.transform.Rotate(new Vector3(0, 0, 1), stepRotZ);
                rotatorY.transform.Rotate(new Vector3(0, 1, 0), stepRotY);
                yield return new WaitForSeconds(.05f);
            }
            rotateZBuffer = rotateYBuffer = zoomBuffer = 0;
            currentZoom = maxZoom;
            lastZoom = currentZoom;
        }
    }

    public class ModuleSpyExperiment : ModuleScienceExperiment
    {

        [KSPEvent(guiName = "Deploy", active = true, guiActive = false)]
        public void DeployExperiment()
        {
            base.DeployExperiment();
        }

        [KSPEvent(guiName = "Review Data", active = true, guiActive = true)]
        public void ReviewDataEvent()
        {
            base.ReviewData();
        }
    }
}
