using OLDD_camera.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace OLDD_camera.Utils
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class CameraDestroyer : MonoBehaviour
    {
        protected void Awake()
        {
            GameEvents.onPartDestroyed.Add(PartCameraDeactivate);
            GameEvents.onVesselDestroy.Add(VesselDestroy);
            GameEvents.onVesselChange.Add(RemoveLines);
        }

        protected void OnDestroy()
        {
            GameEvents.onPartDestroyed.Remove(PartCameraDeactivate);
            GameEvents.onVesselDestroy.Remove(VesselDestroy);
            GameEvents.onVesselChange.Remove(RemoveLines);
        }

        private void VesselDestroy(Vessel vessel)
        {
            foreach (var part in vessel.parts)
                PartCameraDeactivate(part);
        }

        private void PartCameraDeactivate(Part part)
        {
            foreach (var module in part.Modules)
            {
                var kspCamera = module as ICamPart;
                if (kspCamera != null)
                    kspCamera.Deactivate();
            }
        }

        private void RemoveLines(Vessel data)
        {
            Destroy(GameObject.Find("scanningRay"));
            Destroy(GameObject.Find("visibilityRay"));
        }
    }
}
