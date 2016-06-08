using UnityEngine;

namespace DockingCamera
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
    class CameraShaders
    {
        public static Material Get(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.OldTV: return OldTV;
                case ShaderType.NightVisionNoise1: return NightVisionNoise1;
                case ShaderType.Noise: return Noise;
                case ShaderType.NoiseNightVision: return NoiseNightVision;
                case ShaderType.NightVisionClear: return NightVisionClear;
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
        private static int i = 0;
        static Material oldtv = null;
        public static Material OldTV
        {
            get
            {
                if (oldtv == null)
                {
                    oldtv = AssetLoader.matOldTV;
                }
                //var Time = -0.4f;
                //var TimeX = Random.Range(-0.4f, 2f);
                //TimeX += Time.deltaTime;
                ////if (TimeX > 100) TimeX = 0;
                ////oldtv.SetFloat("_Distortion", Offset);
                //oldtv.SetFloat("_TimeX", TimeX);                   
                  return oldtv;
            }
        }
    }
}
