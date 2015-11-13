using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DockingCamera
{
    public class BaseKspCamera
    {
        protected static int windowCount = 0;
        protected RenderTexture renderTexture;
        protected Rect windowPosition;
        public Rect texturePosition;
        protected int windowId;
        protected GameObject partGameObject;
        protected Part part;
        protected string windowLabel;
        protected string subWindowLabel;
        protected Texture textureBackGroundCamera;
        protected Texture textureSeparator;
        protected Texture textureTargetPoint;
        protected Texture textureNoSignal;
        //protected Texture textureBackGroundWindow;
        protected ShaderType shaderType;

        protected float rotateX = 0;
        protected float rotateY = 0;
        protected float rotateZ = 0;

        protected float minZoom = 1f;
        protected float maxZoom = 40f;
        protected float currentZoom = 40f;
        protected bool zoomMultiplier = false;
        protected bool zoomWide = false;
        protected float minZoomMultiplier = 4;

        //protected int userCurrentZoom
        //{
        //    get { return (int)(Mathf.Pow(maxZoom, 2)/Mathf.Pow(currentZoom, 2)); }
        //    set { currentZoom = Mathf.Sqrt(Mathf.Pow(maxZoom, 2)/value); }
        //}

        /// <summary>
        /// 
        /// </summary>
        //protected virtual float ExampleChieldField
        //{
        //    get { return 0; }
        //}


        protected float windowSize = 128f;
        protected int windowSizeCoef = 2;

        public bool IsActivate = false;
        public bool IsAuxiliaryWindowOpen = false;
        public bool IsAuxiliaryWindowButtonPres = false;
        public bool IsButtonOff = false;
        public bool IsOrbital = false;
        //protected bool IsOldTv = false;

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
            GameEvents.OnFlightUIModeChanged.Add(FlightUIModeChanged);
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
            if (windowPosition != null)
            {
                windowPosition.width = windowSize * windowSizeCoef;
                windowPosition.height = windowSize * windowSizeCoef + 32f; //38f;
            }
        }
        /// <summary>
        /// Initializes texture
        /// </summary>
        protected virtual void InitTextures()
        {
            texturePosition = new Rect(6f, 32f, windowPosition.width - 12f, windowPosition.height - 38f); //42f);
            renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.RGB565);  
            RenderTexture.active = renderTexture;
            renderTexture.Create();
            textureBackGroundCamera = Util.MonoColorRectTexture(new Color(0.45f, 0.45f, 0.45f, 1));
            textureSeparator = Util.MonoColorVerticalLineTexture(Color.white, (int)texturePosition.height);
            textureTargetPoint = Util.LoadTexture("targetPoint");
            textureNoSignal = Util.LoadTexture("nosignal");
            //textureBackGroundWindow = Util.MonoColorRectTexture(new Color(0.45f, 0.45f, 0.45f, 0.8f));
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
            var skin = HighLogic.Skin.window.normal.background;
            windowPosition = GUI.Window(windowId, windowPosition, DrawWindow, windowLabel);
        }
        /// <summary>
        /// drawing method
        /// </summary>
        private void DrawWindow(int id)
        {
            //GUI.DrawTexture(new Rect(-1,-1, windowPosition.width+1, windowPosition.height+1), textureBackGroundWindow);
            ExtendedDrawWindowL1();
            ExtendedDrawWindowL2();
            //if (!IsOldTv)
            ExtendedDrawWindowL3();
            GUI.DragWindow();
        }

        /// <summary>
        /// drawing method, first layer, for cameras
        /// </summary>
        protected virtual void ExtendedDrawWindowL1()
        {
            if (!zoomMultiplier) 
                GUI.Label(new Rect(windowPosition.width - 77, windowPosition.height - 25, 75, 20), "zoom: " + (int)(maxZoom-currentZoom+minZoom));
            else
                GUI.Label(new Rect(windowPosition.width - 77, windowPosition.height - 25, 75, 20), "zoom: " + (int)((maxZoom - currentZoom + minZoom))*10);
            Graphics.DrawTexture(texturePosition, textureBackGroundCamera); //background
            Graphics.DrawTexture(texturePosition, Render(), CameraShaders.Get(shaderType));
        }

        /// <summary>
        /// drawing method, second layer (draw any texture between cam and interface)
        /// </summary>
        protected virtual void ExtendedDrawWindowL2()
        {
            //var cameras = Camera.allCameras;
            //var mapCamera = MapView.MapCamera;
            //var mapCamera2 = MapViewFiltering.Instance.camera;
            if (TargetHelper.IsTargetSelect)
            {
                var camera = allCameras.Last();
                //var point = camera.WorldToViewportPoint(TargetHelper.Target.GetTransform().position); //get current target 
                var vessel = TargetHelper.Target as Vessel;
                if (vessel == null)
                {
                    var part = TargetHelper.Target as ModuleDockingNode;
                    vessel = part.vessel;
                }
                var point = camera.WorldToViewportPoint(vessel.transform.position); //get current targeted vessel 
                var x = point.x; // диапазон в (0;1)
                var y = point.y;// диапазон в (0;1)
                var z = point.z;
                if (z > 0 && 0 <= x && x <= 1 && 0 <= y && y <= 1)
                {
                    GUI.DrawTexture(new Rect(texturePosition.xMin + texturePosition.width*x,
                            texturePosition.yMax - texturePosition.height * y,
                            20, 20),
                        textureTargetPoint) ;
                }
            }
            if (IsOrbital)
            {
                Graphics.DrawTexture(texturePosition, textureNoSignal); 
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
                GUI.DrawTexture(new Rect(texturePosition.width+8,32,1,texturePosition.height), textureSeparator);  //vert line, textureSeparator

            if (GUI.Button(new Rect(windowPosition.width - 20, 3, 16, 16), " ")) // destroy cam window
            {
                IsButtonOff = true;
            } 
            if (GUI.Button(new Rect(windowPosition.width - 20, 20, 20, 16), IsAuxiliaryWindowOpen ? "◄" : "►")) //extend window
            {
                IsAuxiliaryWindowOpen = !IsAuxiliaryWindowOpen;
                IsAuxiliaryWindowButtonPres = true;
            }
            if (GUI.Button(new Rect(7, texturePosition.yMax - 20, 20, 20), "☼")) //shader switch
            {
                shaderType++;
                if (!Enum.IsDefined(typeof(ShaderType), shaderType))
                    shaderType = ShaderType.None;
            }

            if (GUI.RepeatButton(new Rect(texturePosition.xMax - 10, texturePosition.yMax - 10, 10, 10), "◰") && 
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
            currentZoom = GUI.HorizontalSlider(new Rect(left, 20, 160, 10),
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

        public IEnumerator ResizeWindow()
        {
            IsAuxiliaryWindowButtonPres = false;
            while (true)
            {
                if (IsAuxiliaryWindowOpen)
                {
                    windowPosition.width += 4;
                    if (windowPosition.width >= windowSize*windowSizeCoef + 80)
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

        /// <summary>
        /// Update position and rotation of the cameras
        /// </summary>
        public virtual void Update()
        {
            //if (IsButtonOff)
            //{
            //    Deactivate();
            //    IsButtonOff = false;
            //    return;
            //}
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

        //public IEnumerator DeactivateOldTv(BaseKspCamera self)
        //{
        //    if (self.IsOldTv) yield return null;
        //    self.IsOldTv = true;
        //    var textPos = new Rect(self.texturePosition);
        //    //self.IsActivate = false;
        //    int coef = 100;
        //    var shift = textPos.height / coef / 2;
        //    for (int i = 0; i < coef-1; i++)
        //    {
        //        texturePosition.yMax -= shift;
        //        texturePosition.yMin += shift;
        //        yield return new WaitForSeconds(.0003f);
        //    }
        //    shift = textPos.width / coef / 2;
        //    for (int i = 0; i < coef-1; i++)
        //    {
        //        texturePosition.xMax -= shift;
        //        texturePosition.xMin += shift;
        //        yield return new WaitForSeconds(.0003f);
        //    }
        //    //self.IsActivate = true;
        //    self.Deactivate();
        //    self.IsOldTv = false;
        //}
        //public IEnumerator ActivateOldTv(BaseKspCamera self)
        //{
        //    if (self.IsOldTv) yield return null;
        //    self.IsOldTv = true;
        //    //return OldTv(1, self);
        //    var centr = self.texturePosition.center;
        //    var textPos = new Rect(self.texturePosition);
        //    self.texturePosition = new Rect(centr.x, centr.y, 1, 1);
        //    self.Activate();
        //    //self.IsActivate = false;
        //    int coef = 20;
        //    var shift = textPos.height/coef/2;
        //    for (int i = 0; i < coef; i++)
        //    {
        //        texturePosition.yMax += shift;
        //        texturePosition.yMin -= shift;
        //        yield return new WaitForSeconds(.0003f);
        //    }
        //    shift = textPos.width/coef/2;
        //    for (int i = 0; i < coef; i++)
        //    {
        //        texturePosition.xMax += shift;
        //        texturePosition.xMin -= shift;
        //        yield return new WaitForSeconds(.0003f);
        //    }
        //    self.IsOldTv = false;
        //    //self.IsActivate = true;
        //}
    }
    
}
