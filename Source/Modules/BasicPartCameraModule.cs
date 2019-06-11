using System.Collections;
using OLDD_camera.Camera;
using OLDD_camera.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OLDD_camera.Modules
{
    class BasicPartCameraModule : PartModule, ICamPart
    {
        /// <summary>
        /// Module adds an external camera and gives control over it
        /// </summary>
        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Docking Camera", isPersistant = true),
        UI_Toggle(controlEnabled = true, enabledText = "ON", disabledText = "OFF", scene = UI_Scene.Flight)]
        public bool IsEnabled;

        [KSPField(isPersistant = true)]
        public bool noise;

        [KSPField(isPersistant = true)]
        private bool _crossDPAI;

        [KSPField(isPersistant = true)]
        private bool _crossOLDD;

        [KSPField(isPersistant = true)]
        private bool _targetCrossStock = true;

        private int _windowSize = 256;
        private DockingCamera _camera;

        [KSPField]
        public string dockingNodeName = "";

        [KSPField]
        public string cameraName = "";

        public override void OnStart(StartState state)
        {
            if (state == StartState.Editor || _camera != null) return;

            if (_camera == null)
            {
               
                _camera = new DockingCamera(part, noise, _targetCrossStock, _crossDPAI, _crossOLDD, _windowSize, cameraName, dockingNodeName);
            }
        }

        public override void OnUpdate()
        {
            if (_camera == null) return;

            if (HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().FCS && part.vessel != FlightGlobals.ActiveVessel && IsEnabled)
            {
                var dist = Vector3.Distance(FlightGlobals.ActiveVessel.transform.position, part.vessel.transform.position);
                var treshhold = vessel.vesselRanges.orbit.load;
                if (dist > treshhold * 0.99)
                    _camera.IsButtonOff = true;
            }

            if (_camera.IsButtonOff)
            {
                IsEnabled = false;
                _camera.IsButtonOff = false;
            }

            if (IsEnabled)
                Activate();
            else
                Deactivate();

            noise = _camera.Noise;
            _crossDPAI = _camera.TargetCrossDPAI;
            _crossOLDD = _camera.TargetCrossOLDD;
            _targetCrossStock = _camera.TargetCrossStock;

            if (_camera.IsAuxiliaryWindowButtonPres)
                StartCoroutine(_camera.ResizeWindow());
            if (_camera.IsActive)
                _camera.Update();
        }

        public void Activate()
        {
            if (_camera.IsActive) return;

            _camera.Activate();
            StartCoroutine("WhiteNoise");
        }

        public void Deactivate()
        {
            if (!_camera.IsActive) return;
            StopCoroutine("WhiteNoise");
            _camera.Deactivate();
        }

        private IEnumerator WhiteNoise()
        {
            while (_camera.IsActive)
            {
                _camera.UpdateNoise();
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}
