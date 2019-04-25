using UnityEngine;

namespace OLDD_camera.Utils
{
    public enum ShaderType
    {
        None = 0,
        CRT = 1,              // Custom/CRT
        NightVision = 2,  // Hidden/NightVision
        NightVisionClear = 3,   // NightVisionClear
        Grayscale = 4,          // Custom/MovieTime
 #if SHADERTEST
        Test = 5,          // none
#endif
       
    }
    public enum ShaderType1
    {
        CRT = 1,
        NightVision = 2,
    }
    public enum ShaderType2
    {
        None = 0,
        NightVisionClear = 3,
        Grayscale = 4,
    }

    internal class CameraShaders
    {
        public static Material GetShader(ShaderType type)
        {
            Debug.Log("GetShader, type: " + type);
#if false
            Material result;
            switch (type)
            {
                case ShaderType.CRT:
                    result= CRT;
                    break;
                case ShaderType.NightVision:
                    result = NightVision;
                    break;
#if false
                case ShaderType.Test:
                    result = TestMaterial;
                    break;
#endif
                case ShaderType.NightVisionClear:
                    result = NightVisionClear;
                    break;
                case ShaderType.Grayscale:
                    result = Grayscale;
                    break;
                case ShaderType.None:
                    result = null;
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
#endif
            switch (type)
            {
                case ShaderType.CRT: return CRT;
                case ShaderType.NightVision: return NightVision;                    
                case ShaderType.NightVisionClear: return NightVisionClear;
                case ShaderType.Grayscale: return Grayscale;
                case ShaderType.None: return null;
#if SHADERTEST
                case ShaderType.Test: return TestMaterial;
#endif
                default: return null;
            }
        }
        public static Material GetShader1(ShaderType1 type)
        {
#if false
            Material result;
            if (type != ShaderType1.CRT)
                result = type != ShaderType1.NightVision ? null : NightVision;
            else
                result = CRT;
            return result;
#endif
            switch (type)
            {
                case ShaderType1.CRT: return CRT;
                case ShaderType1.NightVision: return NightVision;

                default: return null;
            }
        }

        public static Material GetShader2(ShaderType2 type)
        {
#if false
            Material result;
            switch (type)
            {
                case ShaderType2.None:
                    result = null;
                    break;
                case ShaderType2.Grayscale:
                    result = Grayscale;
                    break;
                case ShaderType2.NightVisionClear:
                    result = NightVisionClear;
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
#endif
            switch (type)
            {
                case ShaderType2.None: return null;
                case ShaderType2.NightVisionClear: return NightVisionClear;
                case ShaderType2.Grayscale: return Grayscale;
                default: return null;
            }
        }

        static Material crt = null;
        static Material grayscale = null;
        static Material nightvisionclear = null;
        static Material nightvision = null;
#if SHADERTEST
        static Material testMaterial = null;
#endif
        public static Material CRT
        {
            get
            {
                if (crt == null)
                    crt = AssetLoader.matCRT;
                return crt;
            }
        }

        public static Material Grayscale
        {
            get
            {
                if (grayscale == null)
                    grayscale = AssetLoader.matGrayscale; //new Material(Shader.Find("Hidden/Grayscale Effect"));
                return grayscale;
            }
        }

        public static Material NightVisionClear
        {
            get
            {
                if (nightvisionclear == null)
                    nightvisionclear = AssetLoader.matNightVisionClear;
                return nightvisionclear;
            }
        }

        public static Material NightVision
        {
            get
            {
                if (nightvision == null)
                    nightvision = AssetLoader.matNightVision;
                return nightvision;
            }
        }
#if SHADERTEST
        public static Material TestMaterial
        {
            get
            {
                if (testMaterial == null)
                    testMaterial = AssetLoader.matTest;
                return testMaterial;
            }
        }
#endif

    }
}
