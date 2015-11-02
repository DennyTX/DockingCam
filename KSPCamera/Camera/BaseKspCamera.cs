using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPCamera
{
    public class BaseKspCamera
    {
        protected static int windowCount = 0;
        protected RenderTexture renderTexture;
        protected Rect windowPosition;
        protected Rect texturePosition;
        protected int windowId;
        protected GameObject partGameObject;
        protected Part part;
        protected string windowLabel;
        protected string subWindowLabel;
        protected Texture bg;
        ShaderType shaderType;

        protected int rotateX = 0;
        protected int rotateY = 0;
        protected int rotateZ = 0;

        protected float minZoom = 2;
        protected float maxZoom = 80;
        protected float currentZoom = 40;

        protected float windowSize = 128f;
        protected int windowSizeCoef = 2;

        public bool IsActivate = false;

        protected List<Camera> allCameras = new List<Camera>();
        protected List<GameObject> allCamerasGameObject = new List<GameObject>();
        protected List<string> cameraNames = new List<string>{"GalaxyCamera", "Camera ScaledSpace", "Camera 01", "Camera 00" };

        public BaseKspCamera(Part part, string windowLabel = "Camera")
        {
            this.part = part;
            subWindowLabel = windowLabel;
            this.windowLabel = windowLabel;
            partGameObject = part.gameObject;
            InitWindow();
            InitTextures();
            windowCount++;
        }
        /// <summary>
        /// Initializes window
        /// </summary>
        protected virtual void InitWindow()
        {
            windowId = UnityEngine.Random.Range(1000, 10000);
            if (windowPosition != null)
            {
                windowPosition.width = windowSize * windowSizeCoef;
                windowPosition.height = windowSize * windowSizeCoef + 10f;
            }
        }
        /// <summary>
        /// Initializes texture
        /// </summary>
        protected virtual void InitTextures()
        {

            texturePosition = new Rect(7f, 18f, windowPosition.width - 14f, windowPosition.height - 24f); // position texture for cameras
            renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.RGB565);
            RenderTexture.active = renderTexture;
            renderTexture.Create();
            bg = Util.MonoColorRectTexture(new Color(0.45f, 0.45f, 0.45f, 1));
        }
        /// <summary>
        /// Initializes camera
        /// </summary>
        protected virtual void InitCameras()
        {
            allCamerasGameObject = cameraNames.Select(a => new GameObject()).ToList();
            allCameras = allCamerasGameObject.Select((go, i) =>
                {
                    var camera = go.AddComponent<Camera>();
                    var cameraExample = Camera.allCameras.FirstOrDefault(cam => cam.name == cameraNames[i]);
                    if (cameraExample != null)
                    {
                        camera.CopyFrom(cameraExample);
                        camera.name = string.Format("{1} copy of {0}", cameraNames[i], windowCount);
                        camera.targetTexture = renderTexture;
                    }
                    return camera;
                }).ToList();
        }
        /// <summary>
        /// Destroy Cameras
        /// </summary>
        protected virtual void DestroyCameras()
        {
            allCameras.ForEach(Camera.Destroy);
            allCameras = new List<Camera>();
        }
        /// <summary>
        /// Create and activate cameras
        /// </summary>
        public virtual void Activate()
        {
            if (IsActivate) return;
            InitCameras();
            RenderingManager.AddToPostDrawQueue(0, CamGui);
            IsActivate = true;
        }
        /// <summary>
        /// Destroy  cameras
        /// </summary>
        public virtual void Deactivate()
        {
            if (!IsActivate) return;
            DestroyCameras();
            RenderingManager.RemoveFromPostDrawQueue(0, CamGui);
            IsActivate = false;
        }
        private void CamGui()  //main window
        {
            windowPosition = GUI.Window(windowId, windowPosition, DrawWindow, windowLabel);
        }
        /// <summary>
        /// drawing method
        /// </summary>
        private void DrawWindow(int id)
        {
            Graphics.DrawTexture(texturePosition, bg);
            ExtendedDrawWindowL1();
            ExtendedDrawWindowL2();
            ExtendedDrawWindowL3();
            GUI.DragWindow();
        }

        /// <summary>
        /// drawing method, first layer
        /// </summary>
        protected virtual void ExtendedDrawWindowL1()
        {
            Graphics.DrawTexture(texturePosition, Render(), CameraShaders.Get(shaderType));
        }

        /// <summary>
        /// drawing method, second layer
        /// </summary>
        protected virtual void ExtendedDrawWindowL2()
        {

        }

        /// <summary>
        /// drawing method, third layer
        /// </summary>
        protected virtual void ExtendedDrawWindowL3()
        {
            if (GUI.Button(new Rect(7, texturePosition.yMax - 20, 20, 20), "☼"))
            {
                shaderType++;
                if (!Enum.IsDefined(typeof(ShaderType), shaderType))
                    shaderType = ShaderType.None;
            }
            if (GUI.RepeatButton(new Rect(texturePosition.xMax - 10, texturePosition.yMax - 10, 9, 9), " "))
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
                //windowSizeCoef = windowSizeCoef == 1f ? 2f : 1f;
                Deactivate();
                InitWindow();
                InitTextures();
                Activate();

            }
            var left = windowPosition.width / 2 - 50;
            currentZoom = GUI.HorizontalSlider(new Rect(left, 25, 100, 10),
                currentZoom,
                maxZoom,
                minZoom
                );
        }
        /// <summary>
        /// render texture camera
        /// </summary>
        protected virtual RenderTexture Render()
        {
            allCameras.ForEach(a => a.Render());
            return renderTexture;
        }

        /// <summary>
        /// Update position and rotation of the cameras
        /// </summary>
        public virtual void Update()
        {
            allCamerasGameObject.Last().transform.position = partGameObject.transform.position;
            allCamerasGameObject.Last().transform.rotation = partGameObject.transform.rotation;
            allCamerasGameObject.Last().transform.Rotate(new Vector3(-1f, 0, 0), 90);

            allCamerasGameObject.Last().transform.Rotate(new Vector3(0, 1f, 0), rotateY);
            allCamerasGameObject.Last().transform.Rotate(new Vector3(1f, 0, 0), rotateX);
            allCamerasGameObject.Last().transform.Rotate(new Vector3(0, 0, 1f), rotateZ);

            allCamerasGameObject[0].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[1].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[2].transform.rotation = allCamerasGameObject.Last().transform.rotation;
            allCamerasGameObject[2].transform.position = allCamerasGameObject.Last().transform.position;
            allCameras.ForEach(cam => cam.fieldOfView = currentZoom);
        }
    }
    
}
