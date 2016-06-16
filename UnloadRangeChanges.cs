using System.Collections.Generic;
using UnityEngine;

namespace DockingCamera
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class UnloadRangeChanges : MonoBehaviour
    {
        private readonly int _modulePartCameraId = "PartCameraModule".GetHashCode();
        private VesselRanges defaultRanges = PhysicsGlobals.Instance.VesselRangesDefault;
        private readonly VesselRanges.Situation _myRanges = new VesselRanges.Situation(10000, 10000, 2500, 2500);
        internal static List<Vessel> vesselsWithCamera = new List<Vessel>();

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                GameEvents.onVesselCreate.Add(NewVesselCreated);
                GameEvents.onNewVesselCreated.Add(NewVesselCreated);
            }
        }

        private void NewVesselCreated(Vessel data)
        {
            var allVessels = FlightGlobals.Vessels;
            vesselsWithCamera = GetVesselsWithCamera(allVessels);
            
            foreach (var vessel in vesselsWithCamera)
            {
                vessel.vesselRanges.landed = _myRanges;
                //vessel.vesselRanges.flying = _myRanges;
                vessel.vesselRanges.subOrbital = _myRanges;
                vessel.vesselRanges.landed = _myRanges;
                vessel.vesselRanges.escaping = _myRanges;
                vessel.vesselRanges.orbit = _myRanges;
                vessel.vesselRanges.prelaunch = _myRanges;
                vessel.vesselRanges.splashed = _myRanges;
            }
        }

        private void OnDestroy()
        {
            GameEvents.onNewVesselCreated.Remove(NewVesselCreated);
            GameEvents.onVesselCreate.Remove(NewVesselCreated);
        }

        public List<Vessel> GetVesselsWithCamera(List<Vessel> allVessels)
        {
            List<Vessel> vesselsWithCamera = new List<Vessel>();
            foreach (var vessel in allVessels)
            {
                if (vessel.Parts.Count == 0) continue;
                if (vessel.vesselType == VesselType.Debris 
                    || vessel.vesselType == VesselType.Flag 
                    || vessel.vesselType == VesselType.Unknown) continue;
                //var aaa = vessel.Parts;
                foreach (Part part in vessel.Parts)
                {
                    if (part.Modules.Contains(_modulePartCameraId))
                    {
                        if (vesselsWithCamera.Contains(vessel)) continue;
                        vesselsWithCamera.Add(vessel);
                    }
                }
            }
            return vesselsWithCamera;
        }
    }
}


