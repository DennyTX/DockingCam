using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPCamera
{
    /// <summary>
    /// Extended information about the selected target
    /// </summary>
    public class TargetHelper
    {
        /// <summary>
        /// Object of comparison
        /// </summary>
        private GameObject self;
        private Part selfPart;
        public float DX;
        public float DY;
        public float DZ;
        public float SpeedX;
        public float SpeedY;
        public float SpeedZ;
        public float AngleX;
        public float AngleY;
        public float AngleZ;
        public float Destination;
        public bool isMoveToTarget;
        public float SecondsToDock;
        public bool isDockPort = false;
        //private List<bool> moveToTargetSteps = new List<bool>(100);

        /// <param name="from">Object of comparison</param>
        public TargetHelper(Part from)
        {
            selfPart = from;
            self = selfPart.gameObject;
            //for (int i = 0; i < 100; i++)
            //    moveToTargetSteps.Add(false);
        }
        public static ITargetable Target
        {
            get
            {
                return FlightGlobals.fetch.VesselTarget;
            }
        }
        private Transform targetTransform
        {
            get
            {
                return Target.GetTransform();
            }
        }
        public static bool IsTargetSelect
        {
            //get
            //{
            //    return Target != null;
            //}
            get
            {
                return Target != null &&
                    (Target as ModuleDockingNode != null || Target as Vessel != null);
            }
        }
        public void Update()
        {
            DX = targetTransform.position.x - self.transform.position.x;
            DY = targetTransform.position.y - self.transform.position.y;
            DZ = targetTransform.position.z - self.transform.position.z;

            if (Target as ModuleDockingNode != null)
                isDockPort = true;
            else
            {
                isDockPort = false;
            }

            var velocity = Target.GetObtVelocity() - selfPart.vessel.GetObtVelocity();
            SpeedX = (float)velocity.x;
            SpeedY = (float)velocity.y;
            SpeedZ = (float)velocity.z;

            Destination = (float)Math.Sqrt(Math.Pow(DX, 2) + Math.Pow(DY, 2) + Math.Pow(DZ, 2));

            AngleX = AngleAroundVector(-targetTransform.forward, self.transform.up, -self.transform.forward);
            AngleY = AngleAroundVector(-targetTransform.forward, self.transform.up, self.transform.right);
            AngleZ = AngleAroundVector(targetTransform.up, -self.transform.forward, -self.transform.up);

            // dockingLamp- 
            var checkedDevByZero = false;

            try
            {
                SecondsToDock = Destination / velocity.magnitude;
                checkedDevByZero = true;
            }
            catch (DivideByZeroException)
            {
                isMoveToTarget = false;
            }

            if (checkedDevByZero)
            {
                //var timeX = (Mathf.Abs(DX) < .5f && DX * SpeedX < 0) ? SecondsToDock : -DX / SpeedX;
                //var timeY = (Mathf.Abs(DY) < .5f && DY * SpeedY < 0) ? SecondsToDock : -DY / SpeedY;
                //var timeZ = (Mathf.Abs(DZ) < .5f && DZ * SpeedZ < 0) ? SecondsToDock : -DZ / SpeedZ;
                float timeX;
                float timeY;
                if (SpeedX == 0 && Mathf.Abs(DX) < .5f)
                {
                    timeX = SecondsToDock;
                }
                else
                {
                    timeX = (Mathf.Abs(DX) < .5f) ? SecondsToDock : -DX / SpeedX;
                }

                if (SpeedY == 0 && Mathf.Abs(DY) < .5f)
                    timeY = SecondsToDock;
                else
                    timeY = (Mathf.Abs(DY) < .5f) ? SecondsToDock : -DY / SpeedY;

                //var timeZ = (Mathf.Abs(DZ) < .5f && DZ * SpeedZ < 0) ? SecondsToDock : -DZ / SpeedZ;

                isMoveToTarget = Mathf.Abs(SecondsToDock - timeX) < 1 &&
                                 Mathf.Abs(SecondsToDock - timeY) < 1 &&
                                 DZ * SpeedZ < 0;
                //Mathf.Abs(SecondsToDock - timeZ) < 1;
                
            }

        }

        private static float AngleAroundVector(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = Vector3.Cross(c, a);
            var v2 = Vector3.Cross(c, b);
            if (Vector3.Dot(Vector3.Cross(v1, v2), c) < 0)
                return -Vector3.Angle(v1, v2);
            return Vector3.Angle(v1, v2);
        }
    }
}
