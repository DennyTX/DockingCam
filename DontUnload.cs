//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace DockingCamera
//{
//        [KSPAddon(KSPAddon.Startup.Flight, false)]
//        public class DontUnload : MonoBehaviour
//        {
//            Vessel activeVessel;
//            List<Vessel> keepLoaded;
//            float originalLoadDistance;
//            Dictionary<Vessel, float> originalPackDistance;

//            int updateInterval = 5;
//            float distanceMargin = 500;
//            /* The distance at what something can land safely seems to be 10km */
//            //float safeLandDistance = 10000;
//            int ModuleParachuteID = "PartCameraModule".GetHashCode();

//            //GUIText guiText = new GUIText();

//            void Start()
//            {
//                keepLoaded = new List<Vessel>();
//                originalLoadDistance = Vessel.loadDistance;
//                originalPackDistance = new Dictionary<Vessel, float>();
//                StartCoroutine(updateKeepLoaded());
//            }

//            void OnDestroy()
//            {
//                Vessel.loadDistance = originalLoadDistance;
//            }

//            void Update()
//            {
//                //if (activeVessel != FlightGlobals.ActiveVessel)
//                //{
//                    activeVessel = FlightGlobals.ActiveVessel;
//                //}

//                float maxVesselDistance = 0;
//                float vesselDistance = 0;
//                for (int i = 0; i < keepLoaded.Count; i++)
//                {
//                    Vessel vessel = keepLoaded[i];
//                    if (vessel == null)
//                    {
//                        continue;
//                    }
//                    /* calculate largest vessel distance */
//                    vesselDistance = getDistance(vessel);
//                    maxVesselDistance = Mathf.Max(maxVesselDistance, vesselDistance);

//                    /* update distancePackThreshold */
//                    float packDistance = originalPackDistance[vessel];
//                    if (vesselDistance > (packDistance - distanceMargin))
//                    {
//                        /* raise */
//                        vessel.distancePackThreshold = vesselDistance + distanceMargin;
//                    }
//                    else
//                    {
//                        /* keep default */
//                        vessel.distancePackThreshold = packDistance;
//                    }
//                }

//                /* update loadDistance */
//                //if (maxVesselDistance > (originalLoadDistance - distanceMargin))
//                //{
//                    /* raise */
//                Vessel.loadDistance = 5000;//maxVesselDistance + distanceMargin;
//                //}
//                //else
//                //{
//                //    /* keep default */
//                //    Vessel.loadDistance = originalLoadDistance;
//                //}

//                //Debug();
//            }

//            IEnumerator updateKeepLoaded()
//            {
//                while (true)
//                {
//                    /* remove vessels from the keep list */
//                    for (int i = 0; i < keepLoaded.Count; i++)
//                    {
//                        Vessel vessel = keepLoaded[i];
//                        if (vessel == null)
//                        {
//                            keepLoaded.RemoveAt(i);
//                            i--;
//                        }
//                        else if (!checkSituation(vessel))
//                        {
//                            string name = vessel.GetName();
//                            //Debug.LogError("Removing {0} from keep list because {1}", name,
//                            //    vessel.situation);
//                            if (vessel != activeVessel)
//                            {
//                                ScreenMessages.PostScreenMessage(String.Format("{0} {1}", name,
//                                    vessel.situation),
//                                    3, ScreenMessageStyle.UPPER_CENTER);
//                            }
//                            vessel.distancePackThreshold = originalPackDistance[vessel];
//                            keepLoaded.RemoveAt(i);
//                            i--;
//                        }
//                    }
//                    /* add vessels to the keep list */
//                    List<Vessel>.Enumerator enm = FlightGlobals.Vessels.GetEnumerator();
//                    while (enm.MoveNext())
//                    {
//                        Vessel vessel = enm.Current;
//                        if (vessel.loaded && !vessel.packed && checkSituation(vessel))
//                        {
//                            if (!keepLoaded.Contains(vessel))
//                            {
//                                if (vessel.IsControllable || hasParachutes(vessel))
//                                {
//                                    keepLoaded.Add(vessel);
//                                    originalPackDistance[vessel] = vessel.distancePackThreshold;
//                                }
//                            }
//                        }
//                    }

//                    yield return new WaitForSeconds(updateInterval);
//                }
//            }

//            bool hasParachutes(Vessel vessel)
//            {
//                List<Part>.Enumerator enm = vessel.Parts.GetEnumerator();
//                while (enm.MoveNext())
//                {
//                    Part part = enm.Current;
//                    if (part.Modules.Contains(ModuleParachuteID))
//                    {
//                        return true;
//                    }
//                }
//                return false;
//            }

//            float getDistance(Vessel vessel)
//            {
//                return Vector3.Distance(activeVessel.transform.position, vessel.transform.position);
//            }

//            float getRelativeSpeed(Vessel vessel)
//            {
//                return (activeVessel.GetSrfVelocity() - vessel.GetSrfVelocity()).magnitude;
//            }

//            bool checkSituation(Vessel vessel)
//            {
//                switch (vessel.situation)
//                {
//                    case Vessel.Situations.FLYING:
//                        return true;
//                    default:
//                        return false;
//                }
//            }

//            /* 
//         * Debug methods
//         */

//            //bool debug = false;
//            //Dictionary<Vessel, float> dTerrain = new Dictionary<Vessel, float>();

//            //[Conditional("DEBUG")]
//            //void dPrint(string s, params object[] values)
//            //{
//            //    print(String.Format(s, values));
//            //}

//            //[Conditional("DEBUG")]
//            //void Debug()
//            //{
//            //    if (Input.GetKeyDown(KeyCode.F11))
//            //    {
//            //        debug = !debug;
//            //        if (guiText != null)
//            //        {
//            //            guiText.enabled = debug;
//            //        }
//            //    }
//            //    debugShowVesselInfo();
//            //}

//            //[Conditional("DEBUG")]
//            //void debugShowVesselInfo()
//            //{
//            //    if (!debug)
//            //    {
//            //        return;
//            //    }
//            //    List<Vessel> vessels = FlightGlobals.Vessels;
//            //    List<string> debugInfo = new List<string>();
//            //    string name;
//            //    for (int i = 0; i < vessels.Count; i++)
//            //    {
//            //        Vessel vessel = vessels[i];
//            //        if (!vessel.loaded)
//            //        {
//            //            continue;
//            //        }
//            //        if (keepLoaded.Contains(vessel))
//            //        {
//            //            name = String.Format("<b><color=red>{0}</color>{1}</b>", vessel.GetName(),
//            //                vessel == activeVessel ? " (active)" : "");
//            //        }
//            //        else
//            //        {
//            //            name = vessel.GetName();
//            //        }
//            //        float distance = getDistance(vessel);
//            //        /* this is for figure more or less at what distance the ground isn't safe to land */
//            //        float terrain = 0f;
//            //        if (vessel != activeVessel)
//            //        {
//            //            if (vessel.GetHeightFromTerrain() == -1)
//            //            {
//            //                if (!dTerrain.TryGetValue(vessel, out terrain))
//            //                {
//            //                    dTerrain[vessel] = distance;
//            //                    terrain = distance;
//            //                }
//            //            }
//            //            else
//            //            {
//            //                dTerrain.Remove(vessel);
//            //            }
//            //        }
//            //        debugInfo.Add(String.Format("{0}\n{1}\n" +
//            //                                    "dis: {4:F2} rel spd: {9:F2}\n" +
//            //                                    "ldis: {5:F2} pdis: {7:F2}\n" +
//            //                                    "alt: {2:F2} spd: {3:F2}\n" +
//            //                                    "srf alt: {8:F2} ({10})\n" +
//            //                                    "packed: {6}",
//            //            name,
//            //            vessel.situation,
//            //            vessel.altitude,
//            //            vessel.GetSrfVelocity().magnitude,
//            //            distance,
//            //            Vessel.loadDistance,
//            //            vessel.packed,
//            //            vessel.distancePackThreshold,
//            //            vessel.GetHeightFromTerrain(),
//            //            getRelativeSpeed(vessel),
//            //            terrain));
//            //    }
//            //    if (guiText == null)
//            //    {
//            //        gameObject.AddComponent<GUIText>();
//            //        guiText.transform.position = new Vector3(0.82f, 0.94f, 0f);
//            //        guiText.richText = true;
//            //    }
//            //    guiText.text = String.Join("\n\n", debugInfo.ToArray());
//            //}
//        }
//    }

