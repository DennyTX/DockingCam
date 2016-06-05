using UnityEngine;

namespace DockingCamera
{
    public enum ShaderType
    {
        Noise,
        NoiseNightVision,
        NightVisionClear,
        NightVisionNoise1,
        NightVisionNoise2,
        Grayscale,
        None
    }
    class CameraShaders
    {
        public static Material Get(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.Noise: return Noise;
                case ShaderType.NoiseNightVision: return NoiseNightVision;
                case ShaderType.NightVisionClear: return NightVisionClear;
                case ShaderType.NightVisionNoise1: return NightVisionNoise1;
                case ShaderType.NightVisionNoise2: return NightVisionNoise2;
                case ShaderType.Grayscale: return Grayscale;
                case ShaderType.None: return null;
                default: return null;
            }
        }

        static Material grayscale = null;
        public static Material Grayscale
        {
            get
            {
                if (grayscale == null)
                {
                    grayscale = new Material(Shader.Find("Hidden/Grayscale Effect"));
                }
                return grayscale;
            }
        }

        static Material nightvisionclear = null;
        public static Material NightVisionClear
        {
            get
            {
                if (nightvisionclear == null)
                {
                    nightvisionclear = AssetLoader.matNightVisionClear;
   }
                return nightvisionclear;
            }
        }

        static Material nightvisionnoise1 = null;
        public static Material NightVisionNoise1
        {
            get
            {
                if (nightvisionnoise1 == null)
                {
                    nightvisionnoise1 = AssetLoader.matNightVisionNoise1;
                }
                return nightvisionnoise1;
            }
        }

        static Material nightvisionnoise2 = null;
        public static Material NightVisionNoise2
        {
            get
            {
                if (nightvisionnoise2 == null)
                {
                    nightvisionnoise2 = AssetLoader.matNightVisionNoise2;
                }
                return nightvisionnoise2;
            }
        }

        static Material noise = null;
        public static Material Noise
        {
            get
            {
                if (noise == null)
                {
                    noise = AssetLoader.matNoise;
                }
                return noise;
            }
        }

        static Material noisenightvision = null;
        public static Material NoiseNightVision
        {
            get
            {
                if (noisenightvision == null)
                {
                    noisenightvision = AssetLoader.matNoiseNightVision;
                }
                return noisenightvision;
            }
        }
    }
}
