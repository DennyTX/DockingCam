using System.Collections.Generic;
using KSP.IO;
using KSP.UI.Screens;
using OLDD_camera.Camera;
using OLDD_camera.Modules;
using OLDD_camera.Utils;
using ToolbarControl_NS;
using UnityEngine;
using ClickThroughFix;

namespace OLDD_camera
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DockCamToolbarButton : MonoBehaviour
    {
        internal static DockCamToolbarButton instance;
        private static PluginConfiguration _config;
        private static List<Vessel> _vesselsWithCamera = new List<Vessel>();
        private static List<Vessel> _vesselsWithDockingCamera = new List<Vessel>();
        private static List<Vessel> _vesselsWithAttachedCamera = new List<Vessel>();

        private static Rect _lastWindowPosition;
        private static Rect _windowPosition;
        private static readonly VesselRanges DefaultRanges = PhysicsGlobals.Instance.VesselRangesDefault;
        //private readonly VesselRanges.Situation _myRanges = new VesselRanges.Situation(10000, 10000, 2500, 2500);
        private readonly VesselRanges.Situation _myRanges = new VesselRanges.Situation(10000, 12000, 3500, 2000);
        private const int WINDOW_WIDTH = 256;
        private static bool _showWindow;
        private static bool _shadersToUse0 = true;
        private static bool _shadersToUse1;
        private static bool _shadersToUse2;
        //private static bool _dist2500 = true;
        //private static bool _dist9999;
        private bool mainWindowVisible;
        // private readonly string _modulePartCameraId = "PartCameraModule";

        //public static bool FCS;
        static public bool hideUI = false;


        public void Awake()
        {
            instance = this;
            mainWindowVisible = false; 
        }

        public void Start()
        {
            LoadWindowData();
            if (!HighLogic.LoadedSceneIsFlight) return;
            GameEvents.onVesselCreate.Add(NewVesselCreated);
            GameEvents.onVesselChange.Add(NewVesselCreated);
            GameEvents.onVesselLoaded.Add(NewVesselCreated);
            GameEvents.onVesselsUndocking.Add(VesselsUndocked);
            GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);

            GameEvents.onHideUI.Add(onHideUI);
            GameEvents.onShowUI.Add(onShowUI);
        }

        private void OnDestroy()
        {
            GameEvents.onVesselCreate.Remove(NewVesselCreated);
            GameEvents.onVesselChange.Remove(NewVesselCreated);
            GameEvents.onVesselLoaded.Remove(NewVesselCreated);
            GameEvents.onVesselsUndocking.Remove(VesselsUndocked);
            GameEvents.onGUIApplicationLauncherReady.Remove(OnAppLauncherReady);
            GameEvents.onHideUI.Remove(onHideUI);
            GameEvents.onShowUI.Remove(onShowUI);

            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
            toolbarControl = null;
        }
        void onHideUI()
        {
            hideUI = true;
        }
        void onShowUI()
        {
            hideUI = false;
        }
        public void Update()
        {
            _showWindow = mainWindowVisible && HighLogic.LoadedSceneIsFlight && !FlightGlobals.ActiveVessel.isEVA; // && !MapView.MapIsEnabled;
        }

        private void OnGUI()
        {
            if (! hideUI && mainWindowVisible && HighLogic.LoadedSceneIsFlight) // && !MapView.MapIsEnabled)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().useKSPskin)
                {
                    GUI.skin = HighLogic.Skin;
                }
                if (OLDD_camera.Utils.Styles.KspSkin != HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().useKSPskin)
                {
                    OLDD_camera.Utils.Styles.InitStyles();
                }
                OnWindowOLDD();
            }
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
        internal const string MODID = "Docking_Camera_KURS";
        internal const string MODNAME = "Docking Camera KURS";

        private void OnAppLauncherReady()
        {
            if (toolbarControl != null) return;
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ShowMainWindow, HideMainWindow,
                ApplicationLauncher.AppScenes.FLIGHT,
                MODID, 
                "DockingCameraButton",
                "DockingCamKURS/Icons/DockingCamIcon32",
                "DockingCamKURS/Icons/DockingCamIcon",
                MODNAME);
        }

        private static void OnWindowOLDD()
        {
            if (!_showWindow) return;
            _windowPosition.width = WINDOW_WIDTH;
            var vesselsCount = _vesselsWithCamera.Count;
            var height = 20 * vesselsCount;
            _windowPosition.height = 140 + height + 10;
            //if (!HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().useKSPskin)
            {
                _windowPosition.width += 50;
            }

            _windowPosition = Util.ConstrainToScreen(ClickThruBlocker.GUIWindow(BaseCamera.SettingsWinID, _windowPosition, DrawOnWindowOLDD, "KURS Camera Info"), 100);
        }
        private static string vesselStr(int i)
        {
            if (i == 1)
                return "vessel";
            else
                return "vessels";
        }
        private static void DrawOnWindowOLDD(int windowID)
        {
            var checkDist = FlightGlobals.ActiveVessel.vesselRanges.landed.load;
            var unloadDistance = "Unload at: " + FlightGlobals.ActiveVessel.vesselRanges.landed.load;
            GUILayout.Label(unloadDistance, Styles.Label13B);

            var vessels = _vesselsWithCamera;
            vessels.Remove(FlightGlobals.ActiveVessel);
            //DockCamToolbarButton.instance.NewVesselCreated(FlightGlobals.ActiveVessel);

            GUILayout.Label( " " + _vesselsWithAttachedCamera.Count + " " + vesselStr(_vesselsWithAttachedCamera.Count) + " w/camera in range ", Styles.GreenLabel15B);
            GUILayout.Label( " " + _vesselsWithDockingCamera.Count + " " + vesselStr(_vesselsWithDockingCamera.Count) + " w/docking cam in range ", Styles.GreenLabel15B);

            if (vessels.Count > 1)
            {
                foreach (var vessel in vessels)
                {
                    var i = vessels.IndexOf(vessel) + 1;
                    var range = Mathf.Round(Vector3.Distance(vessel.transform.position, FlightGlobals.ActiveVessel.transform.position));
                    var situation = vessel.RevealSituationString();
                    var str = $"{i}. {vessel.vesselName} ({range:N} m) - {situation} ";
                    if (range <= checkDist)
                        GUILayout.Label(str, Styles.GreenLabel13);
                    else
                        GUILayout.Label(str);
                }
            }
            HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showCross =
                GUILayout.Toggle(HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showCross, "Show crosses");
            HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showSummaryData =
                GUILayout.Toggle(HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showSummaryData, "Show summary data");
            HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showData =
                GUILayout.Toggle(HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showData, "Show detailed data");
            HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showDials =
                GUILayout.Toggle(HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().showDials, "Show rotator dials");
            GUI.DragWindow();

            if (_windowPosition.x == _lastWindowPosition.x &&_windowPosition.y == _lastWindowPosition.y) return;
            _lastWindowPosition.x = _windowPosition.x;
            _lastWindowPosition.y = _windowPosition.y;
            SaveWindowData();
        }

        private void VesselsUndocked(Vessel d1, Vessel d2)
        {
            NewVesselCreated(d1);
        }

        private void NewVesselCreated(Vessel data)
        {
            //return;
            var allVessels = FlightGlobals.Vessels;
            _vesselsWithCamera = GetVesselsWithCamera(allVessels);
            if (!HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>()._dist2500)
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
#if false
                    LogRanges("DefaultRanges.subOrbital: " , DefaultRanges.subOrbital);
                    LogRanges("DefaultRanges.landed: " , DefaultRanges.landed);
                    LogRanges("DefaultRanges.escaping: " , DefaultRanges.escaping);
                    LogRanges("DefaultRanges.orbit: " , DefaultRanges.orbit);
                    LogRanges("DefaultRanges.prelaunch: " , DefaultRanges.prelaunch);
                    LogRanges("DefaultRanges.splashed: " , DefaultRanges.splashed);
#endif
                    vessel.vesselRanges.subOrbital = DefaultRanges.subOrbital;
                    vessel.vesselRanges.landed = DefaultRanges.landed;
                    vessel.vesselRanges.escaping = DefaultRanges.escaping;
                    vessel.vesselRanges.orbit = DefaultRanges.orbit;
                    vessel.vesselRanges.prelaunch = DefaultRanges.prelaunch;
                    vessel.vesselRanges.splashed = DefaultRanges.splashed;
                }
            }
        }
#if false
        void LogRanges(string n, VesselRanges.Situation vr)
        {
            Log.Info("LogRanges: " + n + ", load: " + vr.load + ", unload: " + vr.unload + ", pack: " + vr.pack + ", unpack: " + vr.unpack);
        }
#endif
        private List<Vessel> GetVesselsWithCamera(List<Vessel> allVessels)
        {
            List<Vessel> vesselsWithCamera = new List<Vessel>();
            //List<Vessel> _vesselsWithDockingCamera = new List<Vessel>();
            //List<Vessel> _vesselsWithAttachedCamera = new List<Vessel>();

            foreach (var vessel in allVessels)
            {
                if (vessel.Parts.Count == 0) continue;
                if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.Unknown) continue;

                foreach (var part in vessel.Parts)
                {
                    if (!part.Modules.Contains<PartCameraModule>() && !part.Modules.Contains<DockingCameraModule>()) continue;
                    if (vesselsWithCamera.Contains(vessel)) continue;
                    if (part.Modules.Contains<PartCameraModule>() && !_vesselsWithAttachedCamera.Contains(vessel))
                        _vesselsWithAttachedCamera.Add(vessel);
                    if (part.Modules.Contains<DockingCameraModule>() && !_vesselsWithDockingCamera.Contains(vessel))
                        _vesselsWithDockingCamera.Add(vessel);
                    vesselsWithCamera.Add(vessel);
                    break;
                }
                if (_vesselsWithAttachedCamera.Contains(FlightGlobals.ActiveVessel))
                    _vesselsWithAttachedCamera.Remove(FlightGlobals.ActiveVessel);
                if (_vesselsWithDockingCamera.Contains(FlightGlobals.ActiveVessel))
                    _vesselsWithDockingCamera.Remove(FlightGlobals.ActiveVessel);

            }
            return vesselsWithCamera;
        }

        private static void SaveWindowData()
        {
            _config.SetValue("toolbarWindowPosition", _windowPosition);
            _config.SetValue("shadersToUse0", _shadersToUse0);
            _config.SetValue("shadersToUse1", _shadersToUse1);
            _config.SetValue("shadersToUse2", _shadersToUse2);
            //_config.SetValue("FCS", FCS);
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
            //FCS = _config.GetValue("FCS", FCS);
        }
    }
}
