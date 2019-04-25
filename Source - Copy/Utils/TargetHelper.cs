using System;
using UnityEngine;

namespace OLDD_camera.Utils
{
    /// <summary>
    /// Extended information about the selected target
    /// </summary>
    public class TargetHelper
    {
        private readonly GameObject _self;
        private readonly Part _selfPart;
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
        public bool IsMoveToTarget;
        public float SecondsToDock;
        public bool LookForward;
        public float TargetMoveHelpX;
        public float TargetMoveHelpY;

        /// <param name="from">Object of comparison</param>
        public TargetHelper(Part from)
        {
            _selfPart = from;
            _self = _selfPart.gameObject;
        }
        public static ITargetable Target => FlightGlobals.fetch.VesselTarget;

        private Transform targetTransform => Target.GetTransform();

        public static bool IsTargetSelect => Target != null && (Target as ModuleDockingNode != null || Target as Vessel != null);

        public bool IsDockPort => Target is ModuleDockingNode;

        public void Update()
        {
            UpdatePosition();
            var velocity = UpdateSpeed();
            Destination = (float)Math.Sqrt(Math.Pow(DX, 2) + Math.Pow(DY, 2) + Math.Pow(DZ, 2));
            UpdateAngles();
            UpdateIsMoveToTarget(velocity);
            UpdateTargetMoveHelp();
        }

        private void UpdatePosition()
        {
            DX = targetTransform.position.x - _self.transform.position.x;
            DY = targetTransform.position.y - _self.transform.position.y;
            DZ = targetTransform.position.z - _self.transform.position.z;
        }

        private void UpdateIsMoveToTarget(Vector3 velocity)             // dockingLamp- 
        {
            var checkedDevByZero = false;

            try
            {
                SecondsToDock = Destination / velocity.magnitude;
                checkedDevByZero = true;
            }
            catch (DivideByZeroException)
            {
                IsMoveToTarget = false;
            }

            if (!checkedDevByZero) return;

            float timeX;
            float timeY;
            if (SpeedX == 0 && Mathf.Abs(DX) < .5f)
                timeX = SecondsToDock;
            else
                timeX = (Mathf.Abs(DX) < .5f) ? SecondsToDock : -DX / SpeedX;

            if (SpeedY == 0 && Mathf.Abs(DY) < .5f)
                timeY = SecondsToDock;
            else
                timeY = (Mathf.Abs(DY) < .5f) ? SecondsToDock : -DY / SpeedY;

            IsMoveToTarget = Mathf.Abs(SecondsToDock - timeX) < 1 &&
                             Mathf.Abs(SecondsToDock - timeY) < 1 &&
                             DZ * SpeedZ < 0;
        }

        private void UpdateAngles()
        {
            AngleX = SignedAngleAroundVector(-targetTransform.forward, _self.transform.up, -_self.transform.forward);
            AngleY = SignedAngleAroundVector(-targetTransform.forward, _self.transform.up, _self.transform.right);
            AngleZ = SignedAngleAroundVector(targetTransform.up, -_self.transform.forward, -_self.transform.up);
        }

        private Vector3 UpdateSpeed()
        {
            var velocity = Target.GetObtVelocity() - _selfPart.vessel.GetObtVelocity();
            SpeedX = velocity.x;
            SpeedY = velocity.y;
            SpeedZ = velocity.z;
            return velocity;
        }

        private void UpdateTargetMoveHelp()
        {
            Vector3 targetToOwnship = _self.transform.position - targetTransform.position;
            var translationDeviation = new Vector2(
                SignedAngleAroundVector(targetToOwnship, targetTransform.forward.normalized, _self.transform.forward),
                SignedAngleAroundVector(targetToOwnship, targetTransform.forward.normalized, -_self.transform.right));
            LookForward = Math.Abs(translationDeviation.x) < 90;
            float gaugeX = (LookForward ? 1 : -1) * ((translationDeviation.x / 90f)%2);
            float gaugeY = (LookForward ? 1 : -1) * ((translationDeviation.y / 90f)%2);
            float exponent = .75f;

            if (Destination <= 5f)
                exponent = 1f;
            else
                if (Destination < 15f)
                {
                    float toGo = Destination - 5f;
                    float range = 15f - 5f;
                    float lerp = toGo / range;
                    float exponentReduction = 1f - .75f;
                    exponent = 1 - (exponentReduction) * lerp;
                }

            TargetMoveHelpX = (ScaleExponentially(gaugeX, exponent) + 1) / 2f;
            TargetMoveHelpY = (ScaleExponentially(gaugeY, exponent) + 1) / 2f;
        }

        private static float SignedAngleAroundVector(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = Vector3.Cross(c, a);
            var v2 = Vector3.Cross(c, b);
            if (Vector3.Dot(Vector3.Cross(v1, v2), c) < 0)
                return -Vector3.Angle(v1, v2);
            return Vector3.Angle(v1, v2);
        }
        
        private static float ScaleExponentially(float value, float exponent)
        {
            return (float)Math.Pow(Math.Abs(value), exponent) * Math.Sign(value);
        }

    }
}
