using UnityEngine;

namespace KSPCamera
{
    /// <summary>
    /// Destroyer cameras
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    class CameraDestroyer: MonoBehaviour
    {
        bool isInit = false;
        /// <summary>
        /// Subscription events
        /// </summary>
        protected void Awake()
        {
            GameEvents.onPartDestroyed.Add(PartCameraDeactivate);
            GameEvents.onVesselDestroy.Add(VesselDestroy);

        }
        /// <summary>
        /// Destroys cameras on the ship
        /// </summary>
        private void VesselDestroy(Vessel vessel)
        {
            foreach (var part in vessel.parts)
                PartCameraDeactivate(part);
        }

        /// <summary>
        /// Destroy camera on the part
        /// </summary>
        private void PartCameraDeactivate(Part part)
        {
            foreach (var module in part.Modules)
            {
                var kspCamera = module as ICamPart;
                if (kspCamera != null)
                    kspCamera.Deactivate();
            }
        }
    }
}
