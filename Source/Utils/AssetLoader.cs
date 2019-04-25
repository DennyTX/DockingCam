using System.Collections;
using UnityEngine;
using System.IO;

namespace OLDD_camera.Utils 
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]

    public class AssetLoader:MonoBehaviour
    {
        public static Material matNightVisionClear = null;
        public static Material matNightVision = null;

        public static Material matTest = null;
        public static Material matCRT = null;
        public static Material matGrayscale = null;

        public static Texture2D texSelfRot = null;
        public static Texture2D texTargetRot = null;
        public static Texture2D texTargetPoint = null;
        public static Texture2D texLampOn = null;
        public static Texture2D texLampOff = null;
        public static Texture2D texDockingCam = null;

        IEnumerator Start()
        {
            string url = "file://" + KSPUtil.ApplicationRootPath + "GameData/DockingCamKURS/Resources/DockingCam.ksp";
            //            string url = "file://" + KSPUtil.ApplicationRootPath + "GameData/DockingCamKURS/Resources/dockingcamshaders.shaders";
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
            var shaderAssets = bundle.LoadAllAssets<Shader>();
#if false
            foreach (var i in bundle.GetAllAssetNames())
            {
                Debug.Log("assetName: " + i);
            }
#endif
            foreach (var shader in shaderAssets)
            {
#if false
                Debug.Log("b1: " + b1.name);
#endif
                if (shader.name == "Custom/CRT")                
                    matCRT = new Material(shader);

                if (shader.name == "Hidden/NightVision")
                    matNightVision =  new Material(shader);
                if (shader.name == "NightVisionClear")
                    matNightVisionClear = new Material(shader);

                if (shader.name == "Custom/MovieTime")
                    matGrayscale = new Material(shader);

#if SHADERTEST
                if (shader.name == "Custom/NightVision")
                {
                    matTest = new Material(shader);
                }
#endif
            }


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
