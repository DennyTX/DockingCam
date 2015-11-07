using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPCamera
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

        public bool IsToZero;
        public PartCamera(Part part, string rotatorZ, string rotatorY, string zoommer, float stepper, string cap, string cameraName, string windowLabel = "Camera")
            : base(part, windowLabel)
        {
            lastZoom = currentZoom;
            this.stepper = stepper;
            this.rotatorZ = partGameObject.gameObject.GetChild(rotatorZ);
            this.rotatorY = partGameObject.gameObject.GetChild(rotatorY);
            this.zoommer = partGameObject.gameObject.GetChild(zoommer);
            personalCamera = partGameObject.gameObject.GetChild(cameraName);
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
                    rotateStep = 1f;
                }
            else if ((maxZoom - currentZoom + minZoom) <= 20)
                {
                    rotateStep = 0.5f;
                }
            else if ((maxZoom - currentZoom + minZoom) <= 30)
                {
                    rotateStep = 0.25f;
                }
            else if ((maxZoom - currentZoom + minZoom) <= 40)
                {
                    rotateStep = 0.025f;
                }


            GUI.Label(new Rect(windowPosition.width - 77, windowPosition.height - 65, 75, 20), string.Format("rotateZ: {0:F0}" , simplifiedRotateZBuffer));
            GUI.Label(new Rect(windowPosition.width - 77, windowPosition.height - 45, 75, 20), string.Format("rotateY: {0:F0}", rotateYBuffer));
            var width = windowPosition.width-78;
            if (GUI.Button(new Rect(width+5, 20, 22, 22), "↻"))
            {
                personalCamera.transform.Rotate(new Vector3(0, 0, 180f)); 
                //rotateX += 1;
            }
            if (GUI.RepeatButton(new Rect(width + 28, 20, 22, 22), "↑"))
            {
                if (rotateYBuffer < 180)
                    rotateY += rotateStep; //1;
            }
            if (GUI.RepeatButton(new Rect(width + 51, 20, 22, 22), " "))
            {
                rotateX -= 1;
            }
            if (GUI.RepeatButton(new Rect(width + 5, 43, 22, 22), "←"))
            {
                rotateZ -= rotateStep; //0.1f;
            }
            if (GUI.Button(new Rect(width + 28, 43, 22, 22), "o"))
            {
                //rotatorZ.transform.Rotate(new Vector3(0, 0, 1f), -rotateZBuffer);
                //rotatorY.transform.Rotate(new Vector3(0, 1f, 0), -rotateYBuffer);
                IsToZero = true;
                //currentZoom = 40f;
            }
            if (GUI.RepeatButton(new Rect(width + 51, 43, 22, 22), "→"))
            {
                rotateZ += rotateStep; //0.1f;
            }
            if (GUI.RepeatButton(new Rect(width + 5, 66, 22, 22), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom; 
            }
            if (GUI.RepeatButton(new Rect(width + 28, 66, 22, 22), "↓"))
            {
                if (rotateYBuffer > 0)
                    rotateY -= rotateStep; //1;
            }
            if (GUI.RepeatButton(new Rect(width + 51, 66, 22, 22), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }

            base.ExtendedDrawWindowL1();
        }

        public override void Update()
        {
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
            allCameras.ForEach(cam => cam.fieldOfView = currentZoom);
            rotateZ = 0; 
            rotateY = 0;
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
}
