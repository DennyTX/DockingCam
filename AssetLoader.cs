using System.Collections;
using UnityEngine;

namespace DockingCamera 
{
    [KSPAddon(KSPAddon.Startup.EveryScene, true)]

    public class AssetLoader:MonoBehaviour
    {
        public static Material matNightVisionClear = null;
        public static Material matNightVisionNoise1 = null;
        public static Material matNightVisionNoise2 = null;
        public static Material matNoise = null;
        public static Material matNoiseNightVision = null;
        public static Material matOldTV = null;
        public static Texture2D texSelfRot = null;
        public static Texture2D texTargetRot = null;
        public static Texture2D texTargetPoint = null;
        public static Texture2D texLampOn = null;
        public static Texture2D texLampOff = null;
        public static Texture2D texDockingCam = null;

        IEnumerator Start()
        {
            string url = "file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/GameData/OLDD/DockingCam/dockingcameraassets";
            while (!Caching.ready)
                yield return null;
            Debug.Log("AssetLoader::Start caching ready");
            // Start a download of the given URL
            WWW www = new WWW(url);
            // Wait for download to complete
            yield return www;
            // Load and retrieve the AssetBundle
            Debug.Log("AssetLoader:finished");
            AssetBundle bundle = www.assetBundle;
            matNightVisionClear = (Material)bundle.LoadAsset("NightVisionClear");
            matNightVisionNoise1 = (Material)bundle.LoadAsset("NightVisionNoise1");
            matNightVisionNoise2 = (Material)bundle.LoadAsset("NightVisionNoise2");
            matNoise = (Material)bundle.LoadAsset("Noise");
            matNoiseNightVision = (Material)bundle.LoadAsset("NoiseNightVision");
            matOldTV = (Material)bundle.LoadAsset("OldTV");
            texSelfRot = (Texture2D)bundle.LoadAsset("selfrot");
            texTargetRot = (Texture2D)bundle.LoadAsset("targetrot");
            texTargetPoint = (Texture2D)bundle.LoadAsset("targetPoint");
            texLampOn = (Texture2D)bundle.LoadAsset("lampon");
            texLampOff = (Texture2D)bundle.LoadAsset("lampoff");
            texDockingCam = (Texture2D)bundle.LoadAsset("dockingcam");
            Debug.Log("AssetLoader: get all materials");
            www.Dispose();
        }
    }
}
