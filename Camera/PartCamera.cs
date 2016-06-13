using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DockingCamera
{
    class PartCamera:BaseKspCamera
    {
        private static HashSet<int> usedId = new HashSet<int>();

        private static float CurrentX = -32;
        private static float CurrentY = 32;

        //private static GUIStyle guiStyleBold;  
        private GameObject rotatorZ;
        private GameObject rotatorY;
        private GameObject zoommer;
        private GameObject personalCamera;
        private LineRenderer scanningRay;
        private LineRenderer visibilityRay;
        private float stepper;
        private float rotateZBuffer;
        private float rotateYBuffer;
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

        public float realZoom
        {
            get { return (zoomMultiplier ? currentZoom / minZoomMultiplier : currentZoom); }
            set { currentZoom = value; }
        }

        public PartCamera(Part part, string resourceScanning, string bulletName, int _hits, string rotatorZ, string rotatorY, string zoommer,
               float stepper, string cameraName, int allowedDistance, int windowSize, string windowLabel = "Camera")
               : base(part, windowSize, windowLabel)
                {
                    lastZoom = currentZoom;
                    AllowedDistance = allowedDistance; 
                    this.stepper = stepper;
                    this.bulletName = bulletName;

                    var splresource = resourceScanning.Split('.').ToList();
                    ResourceName = (splresource[0]);
                    ResourceUsage = Int32.Parse(splresource[1]); 
            
                    this.rotatorZ = partGameObject.gameObject.GetChild(rotatorZ);
                    this.rotatorY = partGameObject.gameObject.GetChild(rotatorY);
                    this.zoommer = partGameObject.gameObject.GetChild(zoommer);
                    personalCamera = partGameObject.gameObject.GetChild(cameraName);

                    GameEvents.onLevelWasLoaded.Add(LevelWasLoaded);

                    if (_hits == -1)
                    {
                        hits = 0;
                        while (true)
                        {
                            var hit = partGameObject.GetChild(string.Format("{0}{1:000}", bulletName, hits + 1));
                            //var hit = partGameObject.GetChild(string.Format("{0}{1:000}", hitName, hits + 1));
                            if (hit == null)
                                break;
                            hits++;
                        }
                    }
                    else
                    {
                        hits = _hits;
                        var i = hits+1;
                        while (true)
                        {
                            //var aaa = string.Format("{0}{1:000}", bulletName, i);
                            var hit = partGameObject.GetChild(string.Format("{0}{1:000}", bulletName, i)); 
                            //var hit = partGameObject.GetChild(string.Format("{0}{1:000}", hitName, i));
                            if (hit == null)
                                break;
                            GameObject.Destroy(hit);
                            i++;
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
            GameEvents.onLevelWasLoaded.Remove(LevelWasLoaded);
        }

        protected override void ExtendedDrawWindowL1()
        {
            if (IsOrbital) return;
            
            simplifiedRotateZBuffer =  rotateZBuffer;
            if (Mathf.Abs(simplifiedRotateZBuffer) > 360)
            {
                simplifiedRotateZBuffer = simplifiedRotateZBuffer % 360;
            }

            if ((maxZoom - currentZoom + minZoom) <= 10)
                {
                    rotateStep = 0.8f;
                }
            else if ((maxZoom - currentZoom + minZoom) <= 20)
                {
                    rotateStep = 0.4f;
                }
            else if ((maxZoom - currentZoom + minZoom) <= 30)
                {
                    rotateStep = 0.2f;
                }
            else if ((maxZoom - currentZoom + minZoom) <= 40)
                {
                    rotateStep = 0.02f;
                }
            else if (zoomMultiplier)
                {
                    rotateStep = 0.01f;
                }

            //BUTTONS

            var width = windowPosition.width-84;
            var buttonSize = 25;

            if (GUI.Button(new Rect(width, 36, buttonSize, buttonSize), "↻"))
            {
                personalCamera.transform.Rotate(new Vector3(0, 0, 180f));
                IsUpsideDown = !IsUpsideDown;
            }
            if (GUI.RepeatButton(new Rect(width + buttonSize, 36, buttonSize, buttonSize), "↑"))
            {
                if (rotateYBuffer < 180)
                    if (!IsUpsideDown)
                        rotateY += rotateStep; 
                    else
                        rotateY -= rotateStep; 
            }
            if (GUI.Button(new Rect(width + buttonSize*2, 36, buttonSize, buttonSize), "⦿"))
            {
                //isTargetVisiable();
                if (hits <= 0)
                {
                    ScreenMessages.PostScreenMessage("BULLETS DEPLETED", 3f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
                if (!TargetHelper.IsTargetSelect)
                {
                    ScreenMessages.PostScreenMessage("NO TARGET FOR SCANNING", 3f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
                if (HitCounter() && UseResourceForScanning())
                {
                    IsRayEnabled = true;
                    IsWaitForRay = true;
                    IsScienceActivate = false;
                }
            }
            if (GUI.RepeatButton(new Rect(width, 36 + buttonSize, buttonSize, buttonSize), "←"))
            {
                if (!IsUpsideDown)
                    rotateZ -= rotateStep;
                else
                    rotateZ += rotateStep; 
            }
            if (GUI.Button(new Rect(width + buttonSize, 36 + buttonSize, buttonSize, buttonSize), "o"))
            {
                IsToZero = true;
            }
            if (GUI.RepeatButton(new Rect(width + buttonSize * 2, 36 + buttonSize, buttonSize, buttonSize), "→"))
            {
                if (!IsUpsideDown)
                    rotateZ += rotateStep;
                else
                    rotateZ -= rotateStep; 
            }
            if (GUI.Button(new Rect(width, 36 + buttonSize*2, buttonSize, buttonSize), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom; 
            }
            if (GUI.RepeatButton(new Rect(width + buttonSize, 36 + buttonSize*2, buttonSize, buttonSize), "↓"))
            {
                if (rotateYBuffer > 0)
                    if (!IsUpsideDown)
                        rotateY -= rotateStep;
                    else
                        rotateY += rotateStep;
            }
            if (GUI.Button(new Rect(width + buttonSize * 2, 36 + buttonSize*2, buttonSize, buttonSize), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }

            var widthOffset = width - 2;

            zoomMultiplier = GUI.Toggle(new Rect(widthOffset, 116, 77, 20), zoomMultiplier, " x 10");

            GUI.Label(new Rect(widthOffset, 158, 77, 20), string.Format("rotateZ: {0:F0}°", simplifiedRotateZBuffer));
            GUI.Label(new Rect(widthOffset, 178, 77, 20), string.Format("rotateY: {0:F0}°", rotateYBuffer));

            if (GUI.Button(new Rect(widthOffset, 202, 80, 25), "Photo"))
            {
                var PhotoFrom = part.vessel.vesselName; 
                renderTexture.SavePng(PhotoFrom);
            }
            
            IsVisibilityRay = GUI.Toggle(new Rect(widthOffset, 260, 77, 40), IsVisibilityRay, "Target\nRay");

            GUI.Label(new Rect(widthOffset, 305, 77, 20), string.Format("Bullets: {0:F0}", hits));
           
            base.ExtendedDrawWindowL1();
        }

        
        public override void Update()
        {
            UpdateWhiteNoise();
            DrawScanningRay();
            DrawVisibilityRay();

            allCamerasGameObject.Last().transform.position = personalCamera.gameObject.transform.position;
            allCamerasGameObject.Last().transform.rotation = personalCamera.gameObject.transform.rotation;

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
                //var endPoint1 = new Vector3();
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
                visibilityRay.SetPosition(0, personalCamera.transform.position);
                //visibilityRay.SetPosition(0, part.transform.position);
                visibilityRay.SetPosition(1, TargetHelper.Target.GetTransform().position);    
            }
        }

        private bool UseResourceForScanning()
        {
            var res = part.vessel.GetActiveResources().First(x => x.info.name == ResourceName);
            if (res == null)
                return false;
            if (res.amount < ResourceUsage)
            {
                part.RequestResource(ResourceName, ResourceUsage);
                return false;
            }
            part.RequestResource(ResourceName, ResourceUsage);
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

                //var body = FlightGlobals.Bodies[1];
                //var r = body.Radius;
                //var self = part.vessel.GetWorldPos3D();
                //var target = TargetHelper.Target.GetTransform().position;
                //var shift = target - self;
                //var coef = r/Vector3.Distance(self, target);
                //coef *= .5f;
                //shift *= coef;
                //var point = target - shift;
                //var distanceFromPoint = Vector3.Distance(body.position, point);
                ////i++;
                //return distanceFromPoint > r;
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

        public override void Activate()
        {
            if (IsActivate) return;
            SetFreeId();

            windowPosition.x = CurrentX + 32;
            windowPosition.y = CurrentY + 32;
            CurrentX = windowPosition.x;
            CurrentY = windowPosition.y;

            base.Activate();
        }

        public override void Deactivate()
        {
            if (!IsActivate) return;
            //var aaa = usedId.Count;
            windowPosition.x = -32 + 32 * ID;
            windowPosition.y = 32 + 32 * ID;
            windowPosition.x -= 32;
            windowPosition.y -= 32;
            CurrentX = windowPosition.x;
            CurrentY = windowPosition.y;
            usedId.Remove(ID);
            base.Deactivate();
        }

        private void SetFreeId()
        {
            for (int i = 1; i < int.MaxValue; i++)
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
    }

    public class ModuleSpyExperiment: ModuleScienceExperiment
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
