using System.Collections.Generic;
using KSP.IO;
using KSP.UI.Screens;
using OLDD_camera.Camera;
using OLDD_camera.Utils;
using ToolbarControl_NS;
using UnityEngine;

namespace OLDD_camera
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DockCamToolbarButton : MonoBehaviour
    {
        private static PluginConfiguration _config;
        private static List<Vessel> _vesselsWithCamera = new List<Vessel>();
        private static Rect _lastWindowPosition;
        private static Rect _windowPosition;
        private static readonly VesselRanges DefaultRanges = PhysicsGlobals.Instance.VesselRangesDefault;
        private readonly VesselRanges.Situation _myRanges = new VesselRanges.Situation(10000, 10000, 2500, 2500);
        private const int WINDOW_WIDTH = 256;
        private static bool _showWindow;
        private static bool _shadersToUse0 = true;
        private static bool _shadersToUse1;
        private static bool _shadersToUse2;
        private static bool _dist2500 = true;
        private static bool _dist9999;
        private bool mainWindowVisible;
        private readonly int _modulePartCameraId = "PartCameraModule".GetHashCode();

        public static bool FCS;

        public void Awake()
        {
            mainWindowVisible = false; 
        }

        public void Start()
        {
            LoadWindowData();
            if (!HighLogic.LoadedSceneIsFlight) return;
            GameEvents.onVesselCreate.Add(NewVesselCreated);
            GameEvents.onVesselChange.Add(NewVesselCreated);
            GameEvents.onVesselLoaded.Add(NewVesselCreated);
            GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
        }

        private void OnDestroy()
        {
            GameEvents.onVesselCreate.Remove(NewVesselCreated);
            GameEvents.onVesselChange.Remove(NewVesselCreated);
            GameEvents.onVesselLoaded.Remove(NewVesselCreated);
            GameEvents.onGUIApplicationLauncherReady.Remove(OnAppLauncherReady);
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
            toolbarControl = null;
        }

        public void Update()
        {
            _showWindow = mainWindowVisible && HighLogic.LoadedSceneIsFlight && !FlightGlobals.ActiveVessel.isEVA && !MapView.MapIsEnabled;
            if (_shadersToUse0)
                BaseCamera.ShadersToUse = 0;
            else if (_shadersToUse1)
                BaseCamera.ShadersToUse = 1;
            else if (_shadersToUse2)
                BaseCamera.ShadersToUse = 2;
        }

        private void OnGUI()
        {
            if (toolbarControl != null)
                toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<CameraGameSettings>().useBlizzy);
            if (mainWindowVisible)
                OnWindowOLDD();
        }

        private void ShowMainWindow()
        {
            mainWindowVisible = true;
        }

        private void HideMainWindow()
        {
            mainWindowVisible = false;
        }

        private ToolbarControl toolbarControl;
        private void OnAppLauncherReady()
        {
            if (toolbarControl != null) return;
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ShowMainWindow, HideMainWindow,
                ApplicationLauncher.AppScenes.FLIGHT, "OLDD_camera", "DockingCameraButton",
                "OLDD/DockingCam/Icons/DockingCamIcon32",
                "OLDD/DockingCam/Icons/DockingCamIcon",
                "Docking Camera");
            toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<CameraGameSettings>().useBlizzy);
        }

        private static void OnWindowOLDD()
        {
            if (!_showWindow) return;
            _windowPosition.width = WINDOW_WIDTH;
            var vesselsCount = _vesselsWithCamera.Count;
            var height = 20 * vesselsCount;
            _windowPosition.height = 140 + height + 10;
            _windowPosition = Util.ConstrainToScreen(GUI.Window(2222, _windowPosition, DrawOnWindowOLDD, "OLDD Camera Settings"), 100);
        }

        private static void DrawOnWindowOLDD(int windowID)
        {
            var checkDist = FlightGlobals.ActiveVessel.vesselRanges.landed.load;
            if (GUI.Toggle(new Rect(20, 20, 44, 20), _dist2500, "2250"))
            {
                _dist2500 = true;
                _dist9999 = false;
                if (checkDist > 3333 && _dist2500)
                    GameEvents.onVesselChange.Fire(FlightGlobals.ActiveVessel);
            }
            if (GUI.Toggle(new Rect(80, 20, 44, 20), _dist9999, "9999"))
            {
                _dist9999 = true;
                _dist2500 = false;
                if (checkDist < 3333 && _dist9999)
                    GameEvents.onVesselChange.Fire(FlightGlobals.ActiveVessel);
            }
            var unloadDistance = "Unload at: " + FlightGlobals.ActiveVessel.vesselRanges.landed.load;
            GUI.Label(new Rect(140, 20, 100, 20), unloadDistance, Styles.Label13B);

            GetShadersPack();

            if (FCS = GUI.Toggle(new Rect(20, 100, 222, 20), FCS, "Cam shutdown if out of range"))
                SaveWindowData();

            var vessels = _vesselsWithCamera;
            vessels.Remove(FlightGlobals.ActiveVessel);
            GUI.Label(new Rect(2, 120, WINDOW_WIDTH, 24), " " + vessels.Count + " vessels with camera in range ", Styles.GreenLabel15B);

            if (vessels.Count > 1)
            {
                foreach (var vessel in vessels)
                {
                    var i = vessels.IndexOf(vessel) + 1;
                    var range = Mathf.Round(Vector3.Distance(vessel.transform.position, FlightGlobals.ActiveVessel.transform.position));
                    var situation = vessel.RevealSituationString();
                    var str = $"{i}. {vessel.vesselName} ({range:N} m) - {situation} ";
                    if (range <= checkDist)
                        GUI.Label(new Rect(20, 120 + 20 * i, 222, 20), str, Styles.GreenLabel13);
                    else
                        GUI.Label(new Rect(20, 120 + 20 * i, 222, 20), str);
                }
            }

            GUI.DragWindow();

            if (_windowPosition.x == _lastWindowPosition.x &&_windowPosition.y == _lastWindowPosition.y) return;
            _lastWindowPosition.x = _windowPosition.x;
            _lastWindowPosition.y = _windowPosition.y;
            SaveWindowData();
        }

        private static void GetShadersPack()
        {
            if (GUI.Toggle(new Rect(20, 40, 222, 20), _shadersToUse0, "Shaders pack Full (7 choices)"))
            {
                BaseCamera.ShadersToUse = 0;
                _shadersToUse0 = true;
                _shadersToUse1 = false;
                _shadersToUse2 = false;
                SaveWindowData();
            }
            if (GUI.Toggle(new Rect(20, 60, 222, 20), _shadersToUse1, "Shaders pack Noisy (2 choices)"))
            {
                BaseCamera.ShadersToUse = 1;
                _shadersToUse0 = false;
                _shadersToUse1 = true;
                _shadersToUse2 = false;
                SaveWindowData();
            }
            if (GUI.Toggle(new Rect(20, 80, 222, 20), _shadersToUse2, "Shaders pack Standart (3 choices)"))
            {
                BaseCamera.ShadersToUse = 2;
                _shadersToUse0 = false;
                _shadersToUse1 = false;
                _shadersToUse2 = true;
                SaveWindowData();
            }
        }

        private void NewVesselCreated(Vessel data)
        {
            var allVessels = FlightGlobals.Vessels;
            _vesselsWithCamera = GetVesselsWithCamera(allVessels);
            if (!_dist2500)
            {
                foreach (var vessel in _vesselsWithCamera)
                {
                    vessel.vesselRanges.subOrbital = _myRanges;
                    vessel.vesselRanges.landed = _myRanges;
                    vessel.vesselRanges.escaping = _myRanges;
                    vessel.vesselRanges.orbit = _myRanges;
                    vessel.vesselRanges.prelaunch = _myRanges;
                    vessel.vesselRanges.splashed = _myRanges;
                }
            }
            else
            {
                foreach (var vessel in _vesselsWithCamera)
                {
                    vessel.vesselRanges.subOrbital = DefaultRanges.subOrbital;
                    vessel.vesselRanges.landed = DefaultRanges.landed;
                    vessel.vesselRanges.escaping = DefaultRanges.escaping;
                    vessel.vesselRanges.orbit = DefaultRanges.orbit;
                    vessel.vesselRanges.prelaunch = DefaultRanges.prelaunch;
                    vessel.vesselRanges.splashed = DefaultRanges.splashed;
                }
            }
        }

        private List<Vessel> GetVesselsWithCamera(List<Vessel> allVessels)
        {
            List<Vessel> vesselsWithCamera = new List<Vessel>();
            foreach (var vessel in allVessels)
            {
                if (vessel.Parts.Count == 0) continue;
                if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.Unknown) continue;
                foreach (var part in vessel.Parts)
                {
                    if (!part.Modules.Contains(_modulePartCameraId)) continue;
                    if (vesselsWithCamera.Contains(vessel)) continue;
                    vesselsWithCamera.Add(vessel);
                }
            }
            return vesselsWithCamera;
        }

        private static void SaveWindowData()
        {
            _config.SetValue("toolbarWindowPosition", _windowPosition);
            _config.SetValue("shadersToUse0", _shadersToUse0);
            _config.SetValue("shadersToUse1", _shadersToUse1);
            _config.SetValue("shadersToUse2", _shadersToUse2);
            _config.SetValue("FCS", FCS);
            _config.save();
        }

        private static void LoadWindowData()
        {
            _config = PluginConfiguration.CreateForType<DockCamToolbarButton>();
            _config.load();
            var defaultWindow = new Rect(); 
            _windowPosition = _config.GetValue("toolbarWindowPosition", defaultWindow);
            _shadersToUse0 = _config.GetValue("shadersToUse0", _shadersToUse0);
            _shadersToUse1 = _config.GetValue("shadersToUse1", _shadersToUse1);
            _shadersToUse2 = _config.GetValue("shadersToUse2", _shadersToUse2);
            FCS = _config.GetValue("FCS", FCS);
        }
    }
}
