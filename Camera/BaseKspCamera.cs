using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DockingCamera
{
    public class BaseKspCamera
    {
        public Rect texturePosition;
        protected static int windowCount = 0;
        protected string windowLabel;
        protected string subWindowLabel; 
        protected RenderTexture renderTexture;
        protected Rect windowPosition;
        protected GameObject partGameObject;
        protected Part part;
        protected Texture textureBackGroundCamera;
        protected Texture textureSeparator;
        protected Texture textureTargetMark;
        protected Texture[] textureNoSignal;
        protected ShaderType shaderType;

        protected float rotateX = 0;
        protected float rotateY = 0;
        protected float rotateZ = 0;
        protected float minZoom = 1f;
        protected float maxZoom = 40f;
        protected float currentZoom = 40f;
        protected float minZoomMultiplier = 4;

        protected bool zoomWide = false;
        protected bool isTargetPoint = false;
        protected float windowSize = 128f;
        protected int windowSizeCoef = 2;
        protected int windowId;
        protected int textureNoSignalId;

        protected bool ShaderSwitcher = false;
        protected bool zoomMultiplier = false;

        public bool IsActivate = false;
        public bool IsAuxiliaryWindowOpen = false;
        public bool IsAuxiliaryWindowButtonPres = false;
        public bool IsButtonOff = false;
        public bool IsOrbital = false;

        public static double ElectricChargeAmount;

        protected List<Camera> allCameras = new List<Camera>();
        protected List<GameObject> allCamerasGameObject = new List<GameObject>();
        protected List<string> cameraNames = new List<string>{"GalaxyCamera", "Camera ScaledSpace", "Camera 01", "Camera 00" };

		//UPDATE_MARK
		protected UpdateGUIObject updateGUIObject;
        
        public BaseKspCamera(Part part, int windowSize, string windowLabel = "Camera")
        {
            this.windowSize = windowSize/2;
            this.part = part;
            subWindowLabel = windowLabel;
            this.windowLabel = windowLabel;
            partGameObject = part.gameObject;
            InitWindow();
            InitTextures();
            windowCount++;
            GameEvents.OnFlightUIModeChanged.Add(FlightUIModeChanged);
			//UPDATE_MARK
			GameObject updateGUIHolder = new GameObject();
 			updateGUIObject = updateGUIHolder.AddComponent<UpdateGUIObject>(); // добавление компонента на объект
			updateGUIHolder.transform.parent = part.transform;
        }

        ~BaseKspCamera()
        {
            GameEvents.OnFlightUIModeChanged.Remove(FlightUIModeChanged);
        }

        private void Awake()
        {
            
        }
        private void FlightUIModeChanged(FlightUIMode mode)
        {
            if (mode == FlightUIMode.ORBITAL)
            {
                IsOrbital = true;
            }
            else
            {
                IsOrbital = false;
            }
        }
        
        /// <summary>
        /// Initializes window
        /// </summary>
        protected virtual void InitWindow()
        {
            windowId = UnityEngine.Random.Range(1000, 10000);
            if (windowPosition.yMin < 64)
                windowPosition.yMin = 64;
            windowPosition.width = windowSize * windowSizeCoef;
            windowPosition.height = windowSize * windowSizeCoef + 34f; //38f;
        }

        /// <summary>
        /// Initializes texture
        /// </summary>
        protected virtual void InitTextures()
        {
            texturePosition = new Rect(6f, 34f, windowPosition.width - 12f, windowPosition.height - 40f); //42f);
            renderTexture = new RenderTexture((int)windowSize * 4, (int)windowSize * 4, 24, RenderTextureFormat.RGB565);  
            RenderTexture.active = renderTexture;
            renderTexture.Create();
            textureBackGroundCamera = Util.MonoColorRectTexture(new Color(0.45f, 0.45f, 0.45f, 1));
            textureSeparator = Util.MonoColorVerticalLineTexture(Color.white, (int)texturePosition.height);
            textureTargetMark = AssetLoader.texTargetPoint;
            textureNoSignal = new Texture[8];
            for (int i = 0; i < textureNoSignal.Length; i++)
            {
                textureNoSignal[i] = Util.WhiteNoiseTexture((int) texturePosition.width, (int) texturePosition.height, 1f); //Util.LoadTexture("nosignal");
            }
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
            IsActivate = true;
            textureTargetMark = AssetLoader.texTargetPoint;
			//Debug.LogWarning("BaseKspCamera::Activate");
            updateGUIObject.updateGUIFunction += OnPostRender;
        }

        /// <summary>
        /// Destroy  cameras
        /// </summary>
        public virtual void Deactivate()
        {
            if (!IsActivate) return;
            DestroyCameras();
            IsActivate = false;
			//Debug.LogWarning("BaseKspCamera::Deactivate");
            updateGUIObject.updateGUIFunction -= OnPostRender;
        }

        void OnPostRender()
        {
			if (IsActivate)
			{
                windowPosition = GUI.Window(windowId, windowPosition, DrawWindow, windowLabel); //main window
                //CamGui();
                ElectricChargeAmount = part.vessel.GetActiveResources().First(x => x.info.name == "ElectricCharge").amount;
                if (ElectricChargeAmount <= 0)
                {
                    ScreenMessages.PostScreenMessage("NOT ENOUGH ENERGY", 3f, ScreenMessageStyle.UPPER_CENTER);
                    IsButtonOff = true;
                }
                if (HighLogic.LoadedSceneIsFlight && !FlightDriver.Pause)
                    part.RequestResource("ElectricCharge", 0.002);               
			}
		}

        //private void CamGui()  //main window
        //{
        //    windowPosition = GUI.Window(windowId, windowPosition, DrawWindow, windowLabel);
        //}

        #region DRAW LAYERS 

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

        /// <summary>
        /// drawing method, first layer, for cameras
        /// </summary>
        protected virtual void ExtendedDrawWindowL1()
        {
            if (!zoomMultiplier)
                GUI.Label(new Rect(windowPosition.width - 86, 140, 77, 20), "zoom: " + (int)(maxZoom - currentZoom + minZoom));
            else
                GUI.Label(new Rect(windowPosition.width - 86, 140, 77, 20), "zoom: " + (int)((maxZoom - currentZoom + minZoom)) * 10);

            isTargetPoint = GUI.Toggle(new Rect(windowPosition.width - 86, 235, 77, 40), isTargetPoint, "Target\nMark");

            Graphics.DrawTexture(texturePosition, textureBackGroundCamera);
            //Material currentMaterial = CameraShaders.Get(shaderType);
            Graphics.DrawTexture(texturePosition, Render(), CameraShaders.Get(shaderType));
        }

        /// <summary>
        /// drawing method, second layer (draw any texture between cam and interface)
        /// </summary>
        protected virtual void ExtendedDrawWindowL2()
        {
            if (TargetHelper.IsTargetSelect)
            {
                var camera = allCameras.Last();
                var vessel = TargetHelper.Target as Vessel;
                if (vessel == null)
                {
                    var zzz = TargetHelper.Target as ModuleDockingNode;
                    vessel = zzz.vessel;
                }

                var point = camera.WorldToViewportPoint(vessel.transform.position); //get current targeted vessel 
                var x = point.x; //(0;1)
                var y = point.y;
                var z = point.z;
                //var heading = point.normalized.z; 
                //if (x >= 0.96 && z <= 0) x = 0.96f;
                //if (x <= 0 && z <= 0) x = -0.02f;
                //if (y >= 0.96 || z <= 0) y = 0.96f;
                //if (y <= 0 || z <= 0) y = -0.02f;
                var offsetX = texturePosition.width * x;
                var offsetY = texturePosition.height * y;

                if (isTargetPoint)
                {
                    if (z > 0 && 0 <= x && x <= 1 && 0 <= y && y <= 1)
                    {
                        GUI.DrawTexture(new Rect(texturePosition.xMin + offsetX, texturePosition.yMax - offsetY, 20, 20), textureTargetMark);
                    }    
                }
            }
            if (IsOrbital)
            {
                Graphics.DrawTexture(texturePosition, textureNoSignal[textureNoSignalId]); 
            }
        }

        /// <summary>
        /// drawing method, third layer, interface
        /// </summary>
        protected virtual void ExtendedDrawWindowL3()  
        {
            if (!part.vessel.Equals(FlightGlobals.ActiveVessel))
            {
                GUI.Label(new Rect(55, 33, 160, 20), "Translation from: " + part.vessel.vesselName);
            }
            if (IsAuxiliaryWindowOpen)
                GUI.DrawTexture(new Rect(texturePosition.width+8, 34, 1, texturePosition.height), textureSeparator);  //vert line, textureSeparator

            if (GUI.Button(new Rect(windowPosition.width - 20, 3, 15, 15), " ")) // destroy cam window
            {
                IsButtonOff = true;
            } 
            if (GUI.Button(new Rect(windowPosition.width - 29, 18, 24, 15), IsAuxiliaryWindowOpen ? "◄" : "►")) //extend window
            {
                IsAuxiliaryWindowOpen = !IsAuxiliaryWindowOpen;
                IsAuxiliaryWindowButtonPres = true;
            }
            if (GUI.Button(new Rect(8, texturePosition.yMax - 22, 20, 20), "☼")) //shader switch
            {
                shaderType++;
                if (!Enum.IsDefined(typeof(ShaderType), shaderType))
                    shaderType = ShaderType.Noise;
            }

            if (GUI.RepeatButton(new Rect(texturePosition.xMax - 22, texturePosition.yMax - 22, 20, 20), "◰") && 
                Camera.allCameras.FirstOrDefault(x => x.name == "Camera 00") != null) //Size of main window
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
                Deactivate();
                InitWindow();
                InitTextures();
                Activate();
                IsAuxiliaryWindowOpen = false;
            }

            var left = texturePosition.width / 2 - 80;
            currentZoom = GUI.HorizontalSlider(new Rect(left, 20, 160, 10), currentZoom, maxZoom, minZoom);
        }

        #endregion DRAW LAYERS

        /// <summary>
        /// render texture camera
        /// </summary>
        protected virtual RenderTexture Render()
        {
            allCameras.ForEach(a => a.Render());
            return renderTexture;
        }

        public IEnumerator ResizeWindow()
        {
            IsAuxiliaryWindowButtonPres = false;
            while (true)
            {
                if (IsAuxiliaryWindowOpen)
                {
                    windowPosition.width += 4;
                    if (windowPosition.width >= windowSize*windowSizeCoef + 88)
                        break;
                }
                else
                {
                    windowPosition.width -= 4;
                    if (windowPosition.width <= windowSize*windowSizeCoef)
                        break;
                }
                yield return new WaitForSeconds(1/20);
            }
        }

        protected void UpdateWhiteNoise()
        {
            if (!IsOrbital) return;
            textureNoSignalId++;
            if (textureNoSignalId >= textureNoSignal.Length)
                textureNoSignalId = 0;
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
