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
        private float lastZoom;
        public PartCamera(Part part, string rotatorZ, string rotatorY, string zoommer, float stepper, string cap, string cameraName, string windowLabel = "Camera")
            : base(part, windowLabel)
        {
            lastZoom = 2*currentZoom;
            this.stepper = stepper;
            this.rotatorZ = partGameObject.gameObject.GetChild(rotatorZ);
            this.rotatorY = partGameObject.gameObject.GetChild(rotatorY);
            this.zoommer = partGameObject.gameObject.GetChild(zoommer);
            personalCamera = partGameObject.gameObject.GetChild(cameraName);
        }
        protected override void ExtendedDrawWindowL3()
        {
            if (GUI.Button(new Rect(5, 20, 22, 22), "↻"))
            {
                personalCamera.transform.Rotate(new Vector3(0, 0, 180f)); 
                //rotateX += 1;
            }
            if (GUI.RepeatButton(new Rect(28, 20, 22, 22), "↑"))
            {
                if (rotateYBuffer < 180)
                    rotateY += 1;
            } 
            if (GUI.RepeatButton(new Rect(51, 20, 22, 22), " "))
            {
                rotateX -= 1;
            }
            if (GUI.RepeatButton(new Rect(5, 43, 22, 22), "←"))
            {
                rotateZ -= 1;
            }
            if (GUI.RepeatButton(new Rect(28, 43, 22, 22), "o"))
            {
                rotatorZ.transform.Rotate(new Vector3(0, 0, 1f), -rotateZBuffer);
                rotatorY.transform.Rotate(new Vector3(0, 1f, 0), -rotateYBuffer);
                rotateZBuffer = rotateYBuffer = 0;
                currentZoom = 40f;
            } 
            if (GUI.RepeatButton(new Rect(51, 43, 22, 22), "→"))
            {
                rotateZ += 1;
            }
            if (GUI.RepeatButton(new Rect(5, 66, 22, 22), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }
            if (GUI.RepeatButton(new Rect(28, 66, 22, 22), "↓"))
            {
                if (rotateYBuffer > 0)
                    rotateY -= 1;
            } 
            if (GUI.RepeatButton(new Rect(51, 66, 22, 22), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom;
            }

            base.ExtendedDrawWindowL3();
        }

        public virtual void Update()
        {
            allCamerasGameObject.Last().transform.position = personalCamera.gameObject.transform.position;
            allCamerasGameObject.Last().transform.rotation = personalCamera.gameObject.transform.rotation;

            var step = -(lastZoom - currentZoom) / stepper;
            lastZoom = currentZoom;
            zoommer.transform.Translate(new Vector3(step, 0, 0));
            rotatorZ.transform.Rotate(new Vector3(0, 0, 0.5f), rotateZ);
            rotatorY.transform.Rotate(new Vector3(0, 0.5f, 0), rotateY);
            rotateZBuffer += rotateZ;
            rotateYBuffer += rotateY;

            allCamerasGameObject[0].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[1].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[2].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[2].transform.position = allCamerasGameObject.Last().transform.position;
            allCameras.ForEach(cam => cam.fieldOfView = currentZoom);
            rotateZ = 0; 
            rotateY = 0;
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
