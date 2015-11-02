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
        private GameObject zoomer;
        private GameObject personalCamera;
        private float stepper;
        private float rotateZBuffer;
        private float rotateYBuffer;
        private float lastZoom;
        public PartCamera(Part part, string rotatorZ, string rotatorY, string zoomer, float stepper,string cap, string cameraName, string windowLabel = "Camera")
            : base(part, windowLabel)
        {
            lastZoom = 2*currentZoom;
            this.stepper = stepper;
            this.rotatorZ = partGameObject.gameObject.GetChild(rotatorZ);
            this.rotatorY = partGameObject.gameObject.GetChild(rotatorY);
            this.zoomer = partGameObject.gameObject.GetChild(zoomer);
            personalCamera = partGameObject.gameObject.GetChild(cameraName);
        }
        protected override void ExtendedDrawWindowL3()
        {
            if (GUI.RepeatButton(new Rect(5, 20, 25, 25), " "))
            {
                rotateX += 1;
            }
            if (GUI.RepeatButton(new Rect(55, 20, 25, 25), " "))
            {
                rotateX -= 1;
            }
            if (GUI.RepeatButton(new Rect(30, 20, 25, 25), "↑"))
            {
                if (rotateYBuffer < 180)
                    rotateY += 1;
            }
            if (GUI.RepeatButton(new Rect(30, 70, 25, 25), "↓"))
            {
                if (rotateYBuffer > 0)
                    rotateY -= 1;
            }
            if (GUI.RepeatButton(new Rect(5, 45, 25, 25), "←"))
            {
                rotateZ -= 1;
                //if (rotateY < -30)
                //    rotateY += 1; 
            }
            if (GUI.RepeatButton(new Rect(55, 45, 25, 25), "→"))
            {
                rotateZ += 1;
                //if (rotateY > 30)
                //    rotateY -= 1; ;

            }
            if (GUI.RepeatButton(new Rect(5, 70, 25, 25), "+"))
            {
                currentZoom -= 0.5f;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }
            if (GUI.RepeatButton(new Rect(55, 70, 25, 25), "-"))
            {
                currentZoom += 0.5f;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom;
            }
            if (GUI.RepeatButton(new Rect(30, 45, 25, 25) , "0"))
            {

                rotatorZ.transform.Rotate(new Vector3(0, 0, 1f), -rotateZBuffer);
                rotatorY.transform.Rotate(new Vector3(0, 1f, 0), -rotateYBuffer);
                rotateZBuffer = rotateYBuffer = 0;
                currentZoom = 40f;
                
                //lastZoom = currentZoom;
                //rotateX = 0;
                //rotateY = 0;
                //rotateZ = 0;
                //rotatorZ.transform.rotation = rotatorZQuat;
                //rotatorY.transform.rotation = rotatorYQuat;
            }

            base.ExtendedDrawWindowL3();
        }

        public virtual void Update()
        {
            allCamerasGameObject.Last().transform.position = personalCamera.gameObject.transform.position;
            allCamerasGameObject.Last().transform.rotation = personalCamera.gameObject.transform.rotation;
            //allCamerasGameObject.Last().transform.Rotate(new Vector3(-1f, 0, 0), 90);

            var step = (lastZoom - currentZoom) / stepper;
            lastZoom = currentZoom;
            zoomer.transform.Translate(new Vector3(0, 0, step));
            rotatorZ.transform.Rotate(new Vector3(0, 0, 1f), rotateZ);
            rotatorY.transform.Rotate(new Vector3(0, 1f, 0), rotateY);
            rotateZBuffer += rotateZ;
            rotateYBuffer += rotateY;
            //TODO fix unknown rotation
            //allCamerasGameObject.Last().transform.Rotate(new Vector3(1f, 0, 0), rotateX);

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
