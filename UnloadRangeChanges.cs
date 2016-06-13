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
        //    throw new System.NotImplementedException();
        //}
        //private void Update()
        //{

            //PartCameraModule camPart = null;
            //LaunchClamp camClamp = null;
            //bool HasLaunchClamp = false;
            //var aaa = data.Parts;

            //foreach (var part in aaa)
            //{
            //    if (part.Modules.Contains(_modulePartCameraId))
            //    {
            //        camPart = part.GetComponent<PartCameraModule>();
            //    }
            //    if (part.Modules.GetModule<LaunchClamp>() != null)
            //    {
            //        HasLaunchClamp = true;
            //    }
            //}
            //if (HasLaunchClamp && camPart != null)
            //    camPart.lookAtTarget = FlightGlobals.ActiveVessel.transform;

            //var allVessels = FlightGlobals.Vessels;
            //vesselsWithCamera = GetVesselsWithCamera(allVessels);
            
            //if (FlightGlobals.ActiveVessel.vesselName != VesselName)
            //    Destroy(this);
            //VesselRanges range = new VesselRanges();
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
            //initialize();
        }

        //private void initialize()
        //{
        //    print("[CPU Logger] Starting Up CPU Test Run...");
        //    GameEvents.onNewVesselCreated.Add(VesselCreated);
        //    GameEvents.onVesselCreate.Add(VesselCreated);
        //    _mainVessel = FlightGlobals.ActiveVessel;
        //    _mainVessel.vesselRanges = NewRanges(true);
        //}

        private void OnDestroy()
        {
            GameEvents.onNewVesselCreated.Remove(NewVesselCreated);
            GameEvents.onVesselCreate.Remove(NewVesselCreated);
        }

        //private VesselRanges NewRanges(bool plane)
        //{ 
        //    VesselRanges range = new VesselRanges();
        //    VesselRanges.Situation flight = plane ? _planeRanges : _flyingRanges;

        //    range.flying = flight;
        //    range.subOrbital = _suborbitalRanges;
        //    range.landed = _defaultRanges;
        //    range.escaping = _defaultRanges;
        //    range.orbit = _defaultRanges;
        //    range.prelaunch = _defaultRanges;
        //    range.splashed = _defaultRanges;

        //    return range;
        //}

        //private void VesselCreated(Vessel v)
        //{
        //    StartCoroutine(UnloadRange(v));
        //}

        //IEnumerator UnloadRange(Vessel v)
        //{
        //    int timer = 0;
        //    while (timer < 20)
        //    {
        //        timer++;
        //        yield return null;
        //    }
        //        v.vesselRanges = NewRanges(false);
        //        print("[CPU Logger] New Vessel Created - Setting Debris Unload Ranges : [" + v.vesselName + "]");
        //}

        public List<Vessel> GetVesselsWithCamera(List<Vessel> allVessels)
        {
            List<Vessel> vesselsWithCamera = new List<Vessel>();
            foreach (var vessel in allVessels)
            {
                if (vessel.Parts.Count == 0) continue;
                if (vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.Unknown) continue;
                var aaa = vessel.Parts;
                foreach (var part in aaa)
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


