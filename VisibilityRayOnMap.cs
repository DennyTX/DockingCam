using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DockingCamera
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    class VisibilityRayOnMap:MonoBehaviour
    {
        public Texture2D texture;

        public void Awake()
        {
            texture = Util.MonoColorTexture(Color.red, 100, 100);
        }
        public void OnGUI()
        {
            //if (!MapView.MapIsEnabled || !TargetHelper.IsTargetSelect) return;
            if (!MapView.MapIsEnabled || !TargetHelper.IsTargetSelect || !isTargetVisiable()) return;
            
            //var worldPos = ScaledSpace.LocalToScaledSpace(FlightGlobals.ActiveVessel.transform.position);

            //Vector3 pos = MapView.MapCamera.camera.WorldToScreenPoint(worldPos);
            //var screenRect = new Rect((pos.x - 8), (Screen.height - pos.y) - 8, 50, 50);

            //GUI.DrawTexture(screenRect, texture, ScaleMode.ScaleToFit, true);

            //var visibilityRay = new GameObject("line").AddComponent<LineRenderer>();
            //var color = Color.white;
            //visibilityRay.SetColors(color, color);
            //visibilityRay.s
            ////set the number of points to the scanningRay
            //visibilityRay.SetVertexCount(2);
            //visibilityRay.SetWidth(5, 5);
            //visibilityRay.
            ////render visibilityRay
            //visibilityRay.useWorldSpace = false;
            //visibilityRay.SetPosition(0, new Vector3(0,0,0));
            //visibilityRay.SetPosition(1, new Vector3(500, 500, 500));  


            /*var worldPosSelf = ScaledSpace.LocalToScaledSpace(FlightGlobals.ActiveVessel.transform.position);
            Vector3 posSelfTemp = MapView.MapCamera.camera.WorldToScreenPoint(worldPosSelf);
            var worldPosTarget = ScaledSpace.LocalToScaledSpace(FlightGlobals.ActiveVessel.transform.position);
            Vector3 posTarget = MapView.MapCamera.camera.WorldToScreenPoint(worldPosTarget);
            var posSelf = new Vector2(posSelfTemp.x, posSelfTemp.y);*/
        }

        private bool isTargetVisiable()
        {
            if (!TargetHelper.IsTargetSelect) return false;
            foreach (var body in FlightGlobals.Bodies)
            {
                var r = body.Radius;
                var self = FlightGlobals.ActiveVessel.GetWorldPos3D();
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
    }
}
