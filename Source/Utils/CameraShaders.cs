using UnityEngine;

namespace OLDD_camera.Utils
{
    public enum ShaderType
    {
        OldTV,
        NightVisionNoise1,
        Noise,
        NoiseNightVision,
        NightVisionClear,
        Grayscale,
        None
    }
    public enum ShaderType1
    {
        OldTV,
        NightVisionNoise1,
    }
    public enum ShaderType2
    {
        None,
        Grayscale,
        NightVisionClear
    }
    public enum ShaderType3
    {
        Noise,
        NoiseNightVision,
    }
    internal class CameraShaders
    {
        public static Material GetShader(ShaderType type)
        {
            Material result;
            switch (type)
            {
                case ShaderType.OldTV:
                    result = OldTV;
                    break;
                case ShaderType.NightVisionNoise1:
                    result = NightVisionNoise1;
                    break;
                case ShaderType.Noise:
                    result = Noise;
                    break;
                case ShaderType.NoiseNightVision:
                    result = NoiseNightVision;
                    break;
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
            //switch (type)
            //{
            //case ShaderType.OldTV: return OldTV;
            //case ShaderType.NightVisionNoise1: return NightVisionNoise1;
            //case ShaderType.Noise: return Noise;
            //case ShaderType.NoiseNightVision: return NoiseNightVision;
            //case ShaderType.NightVisionClear: return NightVisionClear;
            //case ShaderType.Grayscale: return Grayscale;
            //case ShaderType.None: return null;
            //default: return null;
            //}
        }
        public static Material GetShader1(ShaderType1 type)
        {
            Material result;
            if (type != ShaderType1.OldTV)
                result = type != ShaderType1.NightVisionNoise1 ? null : NightVisionNoise1;
            else
                result = OldTV;
            return result;
            //switch (type)
            //{
            //    case ShaderType1.OldTV: return OldTV;
            //    case ShaderType1.NightVisionNoise1: return NightVisionNoise1;
            //    default: return null;
            //}
        }

        public static Material GetShader2(ShaderType2 type)
        {
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
            //switch (type)
            //{
            //    case ShaderType2.None: return null;
            //    case ShaderType2.NightVisionClear: return NightVisionClear;
            //    case ShaderType2.Grayscale: return Grayscale;
            //    default: return null;
            //}
        }

        static Material oldtv = null;
        static Material grayscale = null;
        static Material nightvisionclear = null;
        static Material nightvisionnoise1 = null;
        static Material noise = null;
        static Material noisenightvision = null;

        public static Material OldTV
        {
            get
            {
                if (oldtv == null)
                    oldtv = AssetLoader.matOldTV;
                return oldtv;
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

        public static Material NightVisionNoise1
        {
            get
            {
                if (nightvisionnoise1 == null)
                    nightvisionnoise1 = AssetLoader.matNightVisionNoise1;
                return nightvisionnoise1;
            }
        }

        public static Material Noise
        {
            get
            {
                if (noise == null)
                    noise = AssetLoader.matNoise;
                return noise;
            }
        }

        public static Material NoiseNightVision
        {
            get
            {
                if (noisenightvision == null)
                    noisenightvision = AssetLoader.matNoiseNightVision;
                return noisenightvision;
            }
        }
    }
}
