using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OLDD_camera.Utils;
using UnityEngine;

namespace OLDD_camera.Camera
{
    public abstract class BaseCamera
    {
        protected static int WindowCount;
        protected static double ElectricChargeAmount;
        public static Material CurrentShader;
        protected UpdateGUIObject UpdateGUIObject;

        internal Rect WindowPosition;
        internal Rect TexturePosition;

        protected string WindowLabel;
        protected string SubWindowLabel; 
        protected GameObject PartGameObject;
        protected Part ThisPart;

        private Texture _textureBackGroundCamera;
        private Texture _textureSeparator;
        private Texture _textureTargetMark;
        internal Texture[] TextureNoSignal;
        internal int TextureNoSignalId;
        protected RenderTexture RenderTexture; 

        private ShaderType _shaderType;
        private ShaderType1 _shaderType1;
        private ShaderType2 _shaderType2;
        private static string _currentShaderName;
        internal static int ShadersToUse = 0;
        
        protected float WindowSize;
        private const float WindowAddition = 92f;
        protected float RotateX = 0;
        protected float RotateY = 0;
        protected float RotateZ = 0;
        
        protected int MinZoomMultiplier = 4; 
        internal float MinZoom = 1f;
        internal float MaxZoom = 32f;
        internal float CurrentZoom = 32f;
        internal int CalculatedZoom;
        internal bool ZoomMultiplier = false; 
        
        private bool _isTargetPoint;

        protected int WindowSizeCoef = 2;
        protected int WindowId = UnityEngine.Random.Range(1000, 10000);

        internal bool IsActive;
        internal bool IsButtonOff;
        internal bool IsOrbital; 
        internal bool IsAuxiliaryWindowOpen;
        internal bool IsAuxiliaryWindowButtonPres;

        protected List<UnityEngine.Camera> AllCameras = new List<UnityEngine.Camera>();
        protected List<GameObject> AllCamerasGameObject = new List<GameObject>();
        protected List<string> CameraNames = new List<string>{"GalaxyCamera", "Camera ScaledSpace", "Camera 01", "Camera 00" };

        protected BaseCamera(Part thisPart, float windowSizeInit, string windowLabel = "Camera")
        {
            WindowSize = windowSizeInit/2;
            ThisPart = thisPart;
            SubWindowLabel = windowLabel;
            WindowLabel = windowLabel;
            PartGameObject = thisPart.gameObject;

            InitWindow();
            InitTextures();

            GameEvents.OnFlightUIModeChanged.Add(FlightUIModeChanged);

            GameObject updateGUIHolder = new GameObject();
            UpdateGUIObject = updateGUIHolder.AddComponent<UpdateGUIObject>();
        }

        ~BaseCamera()
        {
            GameEvents.OnFlightUIModeChanged.Remove(FlightUIModeChanged);
        }

        private void FlightUIModeChanged(FlightUIMode mode)
        {
            IsOrbital = mode == FlightUIMode.ORBITAL;
        }

        protected virtual void InitWindow()
        {
            WindowPosition.width = WindowSize * WindowSizeCoef;
            WindowPosition.height = WindowSize * WindowSizeCoef + 34f;
        }

        protected virtual void InitTextures()
        {
            TexturePosition = new Rect(6, 34, WindowPosition.width - 12f, WindowPosition.height - 40f); //42f);
            RenderTexture = new RenderTexture((int)WindowSize * 4, (int)WindowSize * 4, 24);//, RenderTextureFormat.RGB565);  
            RenderTexture.active = RenderTexture;
            RenderTexture.Create();
            _textureBackGroundCamera = Util.MonoColorRectTexture(new Color(0.45f, 0.45f, 0.45f, 1));
            _textureSeparator = Util.MonoColorVerticalLineTexture(Color.white, (int)TexturePosition.height);
            _textureTargetMark = AssetLoader.texTargetPoint;
            TextureNoSignal = new Texture[8];
            for (int i = 0; i < TextureNoSignal.Length; i++)
            {
                TextureNoSignal[i] = Util.WhiteNoiseTexture((int) TexturePosition.width, (int) TexturePosition.height, 1f); 
            }
        }

        protected virtual void InitCameras()
        {
            AllCamerasGameObject = CameraNames.Select(a => new GameObject()).ToList();
            AllCameras = AllCamerasGameObject.Select((go, i) =>
                {
                    var camera = go.AddComponent<UnityEngine.Camera>();
                    var cameraExample = UnityEngine.Camera.allCameras.FirstOrDefault(cam => cam.name == CameraNames[i]);
                    if (cameraExample != null)
                    {
                        camera.CopyFrom(cameraExample);
                        camera.name = string.Format("{1} copy of {0}", CameraNames[i], WindowCount);
                        camera.targetTexture = RenderTexture;
                    }
                    return camera;
                }).ToList();
        }

        protected virtual void DestroyCameras()
        {
            AllCameras.ForEach(UnityEngine.Object.Destroy);
            AllCameras.Clear();
        }

        /// <summary>
        /// Create and activate cameras
        /// </summary>
        public virtual void Activate()
        {
            if (IsActive) return;
            WindowCount++;
            InitCameras();
            IsActive = true;
            //UpdateGUIObject.UpdateGUIFunction += Begin; //ddd
            UpdateGUIObject.updateGUIFunction += Begin; //lll
        }

        /// <summary>
        /// Destroy  cameras
        /// </summary>
        public virtual void Deactivate()
        {
            if (!IsActive) return;
            WindowCount--;
            DestroyCameras();
            IsActive = false;
            //UpdateGUIObject.UpdateGUIFunction -= Begin; //ddd
            UpdateGUIObject.updateGUIFunction -= Begin; //lll
        }

        private void Begin() //draw main window
        {
            if (!IsActive) return;
            WindowPosition = GUI.Window(WindowId, KSPUtil.ClampRectToScreen(WindowPosition), DrawWindow, WindowLabel); 
            int electricityId = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;
            double electricChargeAmount;
            double electricChargeMaxAmount;
            ThisPart.GetConnectedResourceTotals(electricityId, out electricChargeAmount, out electricChargeMaxAmount);
            if (HighLogic.LoadedSceneIsFlight && !FlightDriver.Pause)
                ThisPart.RequestResource(electricityId, 0.02 * TimeWarp.fixedDeltaTime);
        }

#region DRAW LAYERS 

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
            var widthOffset = WindowPosition.width - 90;
            var calculateZoom = (int)(MaxZoom - CurrentZoom + MinZoom);
            CalculatedZoom = !ZoomMultiplier ? calculateZoom : calculateZoom * MinZoomMultiplier*6;

            GUI.Label(new Rect(widthOffset, 128, 80, 20), "zoom: " + CalculatedZoom, Styles.Label13B);

            if (FlightGlobals.ActiveVessel == ThisPart.vessel) 
                _isTargetPoint = GUI.Toggle(new Rect(widthOffset-2, 233, 88, 20), _isTargetPoint, "Target Mark");

            //GUI.DrawTexture(TexturePosition, _textureBackGroundCamera);

            switch (ShadersToUse)
            {
                case 0:
                    CurrentShader = CameraShaders.GetShader(_shaderType);
                    break;
                case 1:
                    CurrentShader = CameraShaders.GetShader1(_shaderType1);
                    break;
                case 2:
                    CurrentShader = CameraShaders.GetShader2(_shaderType2);
                    break;
            }
            
            _currentShaderName = CurrentShader == null ? "none" : CurrentShader.name;
            Debug.Log("CurrentShader: " + CurrentShader);
            if (Event.current.type.Equals(EventType.Repaint))
                Graphics.DrawTexture(TexturePosition, Render(), CurrentShader);
        }

        /// <summary>
        /// drawing method, second layer (draw any texture between cam and interface)
        /// </summary>
        protected virtual void ExtendedDrawWindowL2()
        {
            if (TargetHelper.IsTargetSelect)
            {
                var camera = AllCameras.Last();
                var vessel = TargetHelper.Target as Vessel;

                if (vessel == null)
                {
                    var targetedDockingNode = TargetHelper.Target as ModuleDockingNode;
                    vessel = targetedDockingNode.vessel;
                }

                var point = camera.WorldToViewportPoint(vessel.transform.position); //get current targeted vessel 
                var x = point.x; //(0;1)
                var y = point.y;
                var z = point.z;
 
                x = GetX(x,z);
                y = GetY(y,z);

                var offsetX = TexturePosition.width * x;
                var offsetY = TexturePosition.height * y;

                if (_isTargetPoint)
                    GUI.DrawTexture(new Rect(TexturePosition.xMin + offsetX-10, TexturePosition.yMax - offsetY-10, 20, 20), _textureTargetMark);
            }

            if (IsOrbital)
                GUI.DrawTexture(TexturePosition, TextureNoSignal[TextureNoSignalId]); 
        }

        /// <summary>
        /// drawing method, third layer, interface
        /// </summary>
        protected virtual void ExtendedDrawWindowL3()  
        {
            if (!ThisPart.vessel.Equals(FlightGlobals.ActiveVessel))
                GUI.Label(new Rect(8, 34, 222, 20), "Broadcast from: " + ThisPart.vessel.vesselName, Styles.GreenLabel11);

            if (IsAuxiliaryWindowOpen)
                GUI.DrawTexture(new Rect(TexturePosition.width + 8, 34, 1, TexturePosition.height), _textureSeparator); //Separator

            if (GUI.Button(new Rect(WindowPosition.width - 20, 3, 15, 15), " "))
                IsButtonOff = true;

            if (GUI.Button(new Rect(WindowPosition.width - 29, 18, 24, 15), IsAuxiliaryWindowOpen ? "◄" : "►")) //extend window
            {
                IsAuxiliaryWindowOpen = !IsAuxiliaryWindowOpen;
                IsAuxiliaryWindowButtonPres = true;
            }

            var tooltip = new GUIContent("☼", _currentShaderName);
            GUI.Box(new Rect(8, TexturePosition.yMax - 22, 20, 20), tooltip);
            GUI.Label(new Rect(64, 128, 200, 40), GUI.tooltip, Styles.GreenLabel15B);
            if (GUI.Button(new Rect(8, TexturePosition.yMax - 22, 20, 20), "☼")) 
            {
                switch (ShadersToUse)
                {
                    case 0:
                        _shaderType++;
                        if (!Enum.IsDefined(typeof (ShaderType), _shaderType))
                            _shaderType = ShaderType.OldTV;
                        break;
                    case 1:
                        _shaderType1++;
                        if (!Enum.IsDefined(typeof(ShaderType1), _shaderType1))
                            _shaderType1 = ShaderType1.OldTV;
                        break;
                    case 2:
                        _shaderType2++;
                        if (!Enum.IsDefined(typeof (ShaderType2), _shaderType2))
                            _shaderType2 = ShaderType2.None;
                        break;
                }
            }

            if (GUI.RepeatButton(new Rect(TexturePosition.xMax - 22, TexturePosition.yMax - 22, 20, 20), "±") && 	
                UnityEngine.Camera.allCameras.FirstOrDefault(x => x.name == "Camera 00") != null) //Size of main window
            {
                switch (WindowSizeCoef)
                {
                    case 2:
                        WindowSizeCoef = 3;
                        break;
                    case 3:
                        WindowSizeCoef = 2; 
                        break;
                }
                Deactivate();
                InitWindow();
                InitTextures();
                Activate();
                IsAuxiliaryWindowOpen = false;
            }

            CurrentZoom = GUI.HorizontalSlider(new Rect(TexturePosition.width / 2 - 80, 20, 160, 10), CurrentZoom, MaxZoom, MinZoom);
        }

#endregion DRAW LAYERS

        private float GetX(float x, float z)
        {
            if (x < 0 && z > 0 && x <= 0) return 0f;
            if (x > 0 && z < 0) return 0f;
            if (x < 0 && z < 0) return 1f;
            if (x > 0 && z > 0 && x >= 1) return 1f;
            return x;
        }
        private float GetY(float y, float z)
        {
            if (z > 0)
            {
                if (y <= 0f) return 0f;
                if (y >= 1f) return 1f;
            }
            if (z < 0)
            {
                if (y <= 0) return 0f;
                if (y >= 1f) return 1f;
            }
            return y;
        }

        protected virtual RenderTexture Render()
        {
            AllCameras.ForEach(a => a.Render());
            return RenderTexture;
        }

        public IEnumerator ResizeWindow()
        {
            IsAuxiliaryWindowButtonPres = false;
            while (true)
            {
                if (IsAuxiliaryWindowOpen && WindowPosition.width < WindowSize * WindowSizeCoef + WindowAddition)
                {
                    WindowPosition.width += 4;
                    if (WindowPosition.width >= WindowSize * WindowSizeCoef + WindowAddition)
                        break;
                }
                else if (WindowPosition.width > WindowSize*WindowSizeCoef)
                {
                    WindowPosition.width -= 4;
                    if (WindowPosition.width <= WindowSize*WindowSizeCoef)
                        break;
                }
                else
                    break;
                //yield return new WaitForSeconds(1f / 92f); //ddd
                yield return new WaitForSeconds(0.0108695654f); //lll
            }
        }

        protected void UpdateWhiteNoise()
        {
            TextureNoSignalId++;
            if (TextureNoSignalId >= TextureNoSignal.Length)
                TextureNoSignalId = 0;
        }

        /// <summary>
        /// Update position and rotation of the cameras
        /// </summary>
        public virtual void Update()
        {
            AllCamerasGameObject.Last().transform.position = PartGameObject.transform.position;
            AllCamerasGameObject.Last().transform.rotation = PartGameObject.transform.rotation;

            AllCamerasGameObject.Last().transform.Rotate(new Vector3(-1f, 0, 0), 90);
            AllCamerasGameObject.Last().transform.Rotate(new Vector3(0, 1f, 0), RotateY);
            AllCamerasGameObject.Last().transform.Rotate(new Vector3(1f, 0, 0), RotateX);
            AllCamerasGameObject.Last().transform.Rotate(new Vector3(0, 0, 1f), RotateZ);

            AllCamerasGameObject[0].transform.rotation = AllCamerasGameObject.Last().transform.rotation;
            AllCamerasGameObject[1].transform.rotation = AllCamerasGameObject.Last().transform.rotation;
            AllCamerasGameObject[2].transform.rotation = AllCamerasGameObject.Last().transform.rotation;
            AllCamerasGameObject[2].transform.position = AllCamerasGameObject.Last().transform.position;
            AllCameras.ForEach(cam => cam.fieldOfView = CurrentZoom);
        }
    }
}
