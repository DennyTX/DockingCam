using System.Collections;
using UnityEngine;
using System.IO;

namespace OLDD_camera.Utils 
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]

    public class AssetLoader:MonoBehaviour
    {
        public static Material matNightVisionClear = null;
        public static Material matNightVisionNoise1 = null;
        //public static Material matNightVisionNoise2 = null;
        public static Material matNoise = null;
        public static Material matNoiseNightVision = null;
        public static Material matOldTV = null;
        public static Material matGrayscale = null;

        public static Texture2D texSelfRot = null;
        public static Texture2D texTargetRot = null;
        public static Texture2D texTargetPoint = null;
        public static Texture2D texLampOn = null;
        public static Texture2D texLampOff = null;
        public static Texture2D texDockingCam = null;

        IEnumerator Start()
        {
            string url = "file://" + KSPUtil.ApplicationRootPath + "GameData/OLDD/DockingCam/Resources/DockingCam.ksp";
//            string url = "file://" + KSPUtil.ApplicationRootPath + "GameData/OLDD/DockingCam/Resources/dockingcamshaders.shaders";
            while (!Caching.ready)
                yield return null;
            Debug.Log("OLDD_AssetLoader: Start caching ready");

            // Start a download of the given URL
            WWW www = new WWW(url);
            // Wait for download to complete
            yield return www;

            // Load and retrieve the AssetBundle
            Debug.Log("OLDD_AssetLoader: finished");
            AssetBundle bundle = www.assetBundle;
            var b = bundle.LoadAllAssets<Shader>();
#if false
            foreach (var i in bundle.GetAllAssetNames())
            {
                Debug.Log("assetName: " + i);
            }
#endif
            foreach (var b1 in b)
            {
                Debug.Log("b1: " + b1.name);
                if (b1.name == "Custom/CRT")                
                    matOldTV = new Material(b1);

                if (b1.name == "Hidden/NightVision")
                    matNightVisionNoise1 =  new Material(b1);
                if (b1.name == "NightVisionClear")
                    matNightVisionClear = new Material(b1);

                if (b1.name == "Custom/MovieTime")
                    matGrayscale = new Material(b1);

                if (b1.name == "Custom/NightVision")
                    matNoiseNightVision = new Material(b1);

                if (b1.name == "MOARdV/Monitor")
                {
                    matNoise = new Material(b1);
                }
            }
#if false
            matGrayscale = (Material)bundle.LoadAsset("Grayscale");
            if (matGrayscale == null)
                Debug.Log("matGrayscale is null");

            matOldTV = Shader.Find("assets/oldtv");

            if (matOldTV == null)
                Debug.Log("matOldTV 1 is null");


            matNightVisionNoise1 = (Material)bundle.LoadAsset("assets/shaders/nightvision1");
            if (matNightVisionNoise1 == null)
                Debug.Log("matNightVisionNoise1 is null");
#endif
#if false
            matNoise = (Material)bundle.LoadAsset("Noise");
            if (matNoise == null)
                Debug.Log("matNoise is null");

            matNoiseNightVision = (Material)bundle.LoadAsset("NoiseNightVision");
            if (matNoiseNightVision == null)
                Debug.Log("matNoiseNightVision is null");

            matNightVisionClear = (Material)bundle.LoadAsset("NightVisionClear");
            if (matNightVisionClear == null)
                Debug.Log("matNightVisionClear is null");

#endif
            texSelfRot = (Texture2D)bundle.LoadAsset("selfrot");
            texTargetRot = (Texture2D)bundle.LoadAsset("targetrot");
            texTargetPoint = (Texture2D)bundle.LoadAsset("targetPoint");     
            texLampOn = (Texture2D)bundle.LoadAsset("lampon");
            texLampOff = (Texture2D)bundle.LoadAsset("lampoff"); 
            texDockingCam = (Texture2D)bundle.LoadAsset("dockingcam");

            www.Dispose();
        }
    }
}
