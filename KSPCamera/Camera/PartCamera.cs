using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DockingCamera
{
    class PartCamera:BaseKspCamera
    {
        private static HashSet<int> usedId = new HashSet<int>();
        private int ID;
        private GameObject rotatorZ;
        private GameObject rotatorY;
        private GameObject zoommer;
        private GameObject personalCamera;
        private float stepper;
        private float rotateZBuffer;
        private float rotateYBuffer;
        private float zoomBuffer;
        private float lastZoom;
        private float simplifiedRotateZBuffer;
        private float rotateStep;
        private bool IsRayEnabled;
        private bool IsUpsideDown;
        private int AllowedDistance;
        private int buttonSize = 24;
        private LineRenderer line;
        private int ResourceUsage = 50;
        private string ResourceName = "ElectricCharge";
        private string hitName = "Sphere";
        public int hits = 4;


        //protected override float ExampleChieldField
        //{
        //    get { return simplifiedRotateZBuffer; }
        //}
        public bool waitRayOn;
        public bool IsToZero;
        public int realZoom
        {
            get { return (int)(zoomMultiplier ? currentZoom / minZoomMultiplier : currentZoom); }
            set { currentZoom = value; }
        }

        //public PartCamera()
        //    :base(null)
        //{
        //}

        public PartCamera(Part part, string rotatorZ, string rotatorY, string zoommer, float stepper, string cap, string cameraName, int allowedDistance, int _hits, string windowLabel = "Camera")
            : base(part, windowLabel)
        {
            lastZoom = currentZoom;
            this.stepper = stepper;
            AllowedDistance = allowedDistance;
            this.rotatorZ = partGameObject.gameObject.GetChild(rotatorZ);
            this.rotatorY = partGameObject.gameObject.GetChild(rotatorY);
            this.zoommer = partGameObject.gameObject.GetChild(zoommer);
            personalCamera = partGameObject.gameObject.GetChild(cameraName);

            if (_hits == -1)
            {
                hits = 0;
                while (true)
                {
                    var hit = partGameObject.GetChild(string.Format("{0}{1:000}", hitName, hits+1));
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
                    var hit = partGameObject.GetChild(string.Format("{0}{1:000}", hitName, i));
                    if (hit == null)
                        break;
                    GameObject.Destroy(hit);
                    i++;
                }
            }
        }

        protected override void ExtendedDrawWindowL1()
        //protected override void ExtendedDrawWindowL3()
        {
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

            var width = windowPosition.width-78;

            if (GUI.Button(new Rect(width + 2, 32, buttonSize, buttonSize), "↻"))
            {
                personalCamera.transform.Rotate(new Vector3(0, 0, 180f));
                IsUpsideDown = !IsUpsideDown;
            }
            if (GUI.RepeatButton(new Rect(width + 25, 32, buttonSize, buttonSize), "↑"))
            {
                if (rotateYBuffer < 180)
                    if (!IsUpsideDown)
                        rotateY += rotateStep; 
                    else
                        rotateY -= rotateStep; 
            }
            if (GUI.Button(new Rect(width + 48, 32, buttonSize, buttonSize), "⦿"))
            {
                
                if (HitCounter() && useEnergy())
                {
                    IsRayEnabled = true;
                    waitRayOn = true;
                    scienceActivate = false;
                }
            }
            if (GUI.RepeatButton(new Rect(width + 2, 55, buttonSize, buttonSize), "←"))
            {
                if (!IsUpsideDown)
                    rotateZ -= rotateStep;
                else
                    rotateZ += rotateStep; 
            }
            if (GUI.Button(new Rect(width + 25, 55, buttonSize, buttonSize), "o"))
            {
                IsToZero = true;
            }
            if (GUI.RepeatButton(new Rect(width + 48, 55, buttonSize, buttonSize), "→"))
            {
                if (!IsUpsideDown)
                    rotateZ += rotateStep;
                else
                    rotateZ -= rotateStep; 
                //rotateZ += rotateStep;
            }
            if (GUI.RepeatButton(new Rect(width + 2, 78, buttonSize, buttonSize), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom; 
            }
            if (GUI.RepeatButton(new Rect(width + 25, 78, buttonSize, buttonSize), "↓"))
            {
                if (rotateYBuffer > 0)
                    if (!IsUpsideDown)
                        rotateY -= rotateStep;
                    else
                        rotateY += rotateStep;
            }
            if (GUI.RepeatButton(new Rect(width + 48, 78, buttonSize, buttonSize), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }

            zoomMultiplier = GUI.Toggle(new Rect(width + 2, 108, 70, 20), zoomMultiplier, " x 10");
            zoomWide = GUI.Toggle(new Rect(width + 2, 128, 70, 20), zoomWide, "Wide");

            GUI.Label(new Rect(windowPosition.width - 77, windowPosition.height - 65, 75, 20), string.Format("rotateZ: {0:F0}", simplifiedRotateZBuffer));
            GUI.Label(new Rect(windowPosition.width - 77, windowPosition.height - 45, 75, 20), string.Format("rotateY: {0:F0}", rotateYBuffer));
            
            base.ExtendedDrawWindowL1();
        }

        public override void Update()
        {
            DrawRay();

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
            //if (zoomMultiplier)
            //    allCameras.ForEach(cam => cam.fieldOfView = currentZoom/minZoomMultiplier);
            //else
            allCameras.ForEach(cam => cam.fieldOfView = realZoom); //currentZoom); 
            rotateZ = 0; 
            rotateY = 0;
        }

        private bool scienceActivate;

        private bool useEnergy()
        {
            var res = part.vessel.GetActiveResources().First(x => x.info.name == ResourceName);
            if (res == null)
                return false;
            if (res.amount < ResourceUsage)
            {
                //res.amount = 0;
                part.RequestResource(ResourceName, ResourceUsage);
                return false;
            }

            part.RequestResource(ResourceName, ResourceUsage);
            //res.amount -= ResourceUsage;
            
            return true;
            //res.amount -= res.amount >= 50 ? 50 : res.amount;
        }
        private bool HitCounter()
        {
            if (hits == 0)
                return false;
            var hit = partGameObject.GetChild(string.Format("{0}{1:000}", hitName, hits));
            GameObject.Destroy(hit);
            hits--;
            return true;
        }
        void DrawRay()
        {
            GameObject.Destroy(line);
            //var ray = new Ray();
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, 100))
            //{
            if (IsRayEnabled && TargetHelper.IsTargetSelect)
            {
                Vector3 endPoint;
                //var endPoint1 = new Vector3();
                if (isInsight(out endPoint))
                {
                    line = new LineRenderer();
                    //create a new empty gameobject and line renderer component
                    line = new GameObject("Line").AddComponent<LineRenderer>();
                    //assign the material to the line
                    line.SetColors(Color.red, Color.red);
                    //set the number of points to the line
                    line.SetVertexCount(2);
                    line.SetWidth(0.02f, 0.02f);
                    //render line to the world origin and not to the object's position
                    line.useWorldSpace = true;
                    line.SetPosition(0, part.transform.position);
                    line.SetPosition(1, endPoint);

                    /////
                    //Ray ray = new Ray(from, to);
                    //RaycastHit hit = new RaycastHit();
                    //if (Physics.Raycast(ray, out hit, 1000))
                    //{
                    //}

                }
            }
            //else
            //{
            //    ScreenMessages.PostScreenMessage("NOTHING TO HIT", 5f, ScreenMessageStyle.UPPER_CENTER);
            //}
            //}
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

        public IEnumerator WaitForRay()
        {
            yield return new WaitForSeconds(1);
            IsRayEnabled = false;
            //var sciencePart = part.vessel.parts.First(a => a.name == "");
            var target = new TargetHelper(part);
            target.Update();
            Vector3 endPoint;
            if (target.Destination <= AllowedDistance && isInsight(out endPoint))
                {
                    ScreenMessages.PostScreenMessage("HIT " + FlightGlobals.activeTarget.vessel.vesselName, 5f, ScreenMessageStyle.UPPER_CENTER); 
                    if (!scienceActivate)
                    {
                        //try
                        //{

                        //    var science = new ModuleScienceExperiment
                        //    {
                        //        experimentID = "gravityScan",
                        //        useStaging = false,
                        //        useActionGroups = true,
                        //        hideUIwhenUnavailable = false,
                        //        xmitDataScalar = 0.4f,
                        //        dataIsCollectable = true,
                        //        interactionRange = 1.2f,
                        //        rerunnable = true,
                        //        usageReqMaskInternal = 1,
                        //        usageReqMaskExternal = 8,
                        //        part = part
                        //    };
                        var spyScience = part.GetComponent<ModuleSpyExperiment>();
                        //var spyScience = part.GetComponent<ModuleScienceExperiment>();
                        //var sciense = sciencePart.GetComponent<ModuleScienceExperiment>();
                        if (spyScience != null)
                            spyScience.DeployExperiment();
                        scienceActivate = true;
                        //    science.DeployExperiment();
                        //    scienceActivate = true;
                        //}
                        //catch (Exception e)
                        //{
                        //    int a = 10*1;
                        //}
                    }
                }
            else
                {
                    ScreenMessages.PostScreenMessage("OBJECT" + FlightGlobals.activeTarget.vessel.vesselName + " IS OUT OF RANGE", 5f, ScreenMessageStyle.UPPER_CENTER);
                }
        }
        public IEnumerator ToZero()
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
            base.Activate();
        }

        public override void Deactivate()
        {
            if (!IsActivate) return;
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
            
        }
    }
}
