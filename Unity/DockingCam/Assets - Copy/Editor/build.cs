using UnityEditor;

public class Bundler
{
	const string bundleName = "dockingcameraassets";
	const string dir = "AssetBundles";
	const string dir2 = "AssetBundles-uncompressed";
	const string extension = ".ksp";

    [MenuItem("DockingCam/Build Bundles")]
    static void BuildAllAssetBundles()
    {
		BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows);
		FileUtil.ReplaceFile(dir + "/" + bundleName, dir + "/DockingCam" + extension);
		
		BuildPipeline.BuildAssetBundles(dir2, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows);
		FileUtil.ReplaceFile(dir2 + "/" + bundleName, dir2 + "/DockingCam" + extension);

		//FileUtil.DeleteFileOrDirectory(dir + "/DockingCam");

        //BuildPipeline.BuildAssetBundles("Bundles-windows", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
		//FileUtil.ReplaceFile("Bundles-windows/Bundles-windows", "Bundles-windows" + "/DockingCam" + extension);
        //BuildPipeline.BuildAssetBundles("Bundles-osx", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneOSXUniversal);
        //BuildPipeline.BuildAssetBundles("Bundles-linux", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneLinuxUniversal);

	}
}

