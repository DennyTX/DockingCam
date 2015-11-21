using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DockingCamera;

namespace DockingCamera
{
     [KSPAddon(KSPAddon.Startup.Flight, false)]
    class MapCamera : MonoBehaviour
    {
        private string _scienceInfoON = "OLDD/ScienceSituationInfo/ScienceInfoON";
        private string _scienceInfoOFF = "OLDD/ScienceSituationInfo/ScienceInfoOFF";
        private static ApplicationLauncherButton _button;


        protected List<Camera> allCameras = new List<Camera>();
        protected List<GameObject> allCamerasGameObject = new List<GameObject>();
        private int windowId;
        private Rect windowPosition;
        protected float windowSize = 128f;
        protected int windowSizeCoef = 2;
        private Rect texturePosition;
        private RenderTexture renderTexture;

        public void Start()
        {

            if (_button == null)
            {
                var texture = GameDatabase.Instance.GetTexture(_scienceInfoOFF, false);
                _button = ApplicationLauncher.Instance.AddModApplication(ButtonTrue, ButtonFalse, () => { }, () => { },
                    () => { }, () => { }, ApplicationLauncher.AppScenes.MAPVIEW
                    , texture);
            }
        }

        private void ButtonTrue()
        {
            _button.SetTexture(GameDatabase.Instance.GetTexture(_scienceInfoON, false));
            RenderingManager.AddToPostDrawQueue(0, CamGui);
            InitWindow();
            InitTextures();
            InitCameras();
        }
        private void ButtonFalse()
        {
            _button.SetTexture(GameDatabase.Instance.GetTexture(_scienceInfoOFF, false));
            RenderingManager.RemoveFromPostDrawQueue(0, CamGui);
        }

        protected void InitWindow()
        {
            windowId = UnityEngine.Random.Range(1000, 10000);
            if (windowPosition.yMin < 64)
                windowPosition.yMin = 64;
            windowPosition.width = windowSize * windowSizeCoef;
            windowPosition.height = windowSize * windowSizeCoef + 32f; //38f;
        }

        protected virtual void InitTextures()
        {
            texturePosition = new Rect(6f, 32f, windowPosition.width - 12f, windowPosition.height - 38f); //42f);
            renderTexture = new RenderTexture((int)windowSize * 4, (int)windowSize * 4, 24, RenderTextureFormat.RGB565);
            RenderTexture.active = renderTexture;
            renderTexture.Create();
            //textureBackGroundCamera = Util.MonoColorRectTexture(new Color(0.45f, 0.45f, 0.45f, 1));
        }

        private void InitCameras()
        {
            var tmp = Camera.allCameras;
            allCamerasGameObject = new List<GameObject>();
            allCameras = new List<Camera>();
            var cameraNames = new List<string>{"GalaxyCamera", "Camera ScaledSpace", "VectorCam", "MapFX Camera 1", "MapFX Camera 2", "UICamera", "UI camera", "UI mask camera"  };
            allCamerasGameObject = cameraNames.Select(a => new GameObject()).ToList();
            allCameras = allCamerasGameObject.Select((go, i) =>
            {
                var camera = go.AddComponent<Camera>();
                var cameraExample = Camera.allCameras.FirstOrDefault(cam => cam.name == cameraNames[i]);
                if (cameraExample != null)
                {
                    camera.CopyFrom(cameraExample);
                    camera.name = string.Format(" {0}", cameraNames[i]);
                    camera.targetTexture = renderTexture;
                }
                return camera;
            }).ToList();

            //var copyFrom = new List<Camera>{MapView.fetch.camera, MapView.fetch.VectorCamera};
            //copyFrom.AddRange(MapView.fetch.uiCameras);
            //foreach (var exampleCamera in copyFrom)
            //{
            //    var go = new GameObject();
            //    allCamerasGameObject.Add(go);
            //    var cam = go.AddComponent<Camera>();
            //    cam.CopyFrom(exampleCamera);
            //    cam.targetTexture = renderTexture;
            //    allCameras.Add(cam);
            //}
        }
        private void CamGui()  //main window
        {
            windowPosition = GUI.Window(windowId, windowPosition, DrawWindow, "Map");
        }
        /// <summary>
        /// drawing method
        /// </summary>
        private void DrawWindow(int id)
        {
            ExtendedDrawWindowL1();
            ExtendedDrawWindowL2();
            ExtendedDrawWindowL3();
            GUI.DragWindow();
        }

        private void ExtendedDrawWindowL3()
        {
            if (GUI.RepeatButton(new Rect(texturePosition.xMax - 10, texturePosition.yMax - 10, 10, 10), "◰") ) //Size of main window
            {
                switch (windowSizeCoef)
                {
                    case 2:
                        windowSizeCoef = 3;
                        break;
                    case 3:
                        windowSizeCoef = 4;
                        break;
                    case 4:
                        windowSizeCoef = 2;
                        break;
                }
                InitWindow();
                InitTextures();
            }
        }

        private void ExtendedDrawWindowL2()
        {
        }

        private void ExtendedDrawWindowL1()
        {
            Graphics.DrawTexture(texturePosition, Render());
        }


        protected virtual RenderTexture Render()
        {
            allCameras.ForEach(a => a.Render());
            return renderTexture;
        }
    }
}
