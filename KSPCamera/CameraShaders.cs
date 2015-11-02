using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPCamera
{
    enum ShaderType
    {
        None,
        Grayscale,
        NightVisison,

    }
    class CameraShaders
    {
        private static string nightVisionArgs = "0.5,0.7,0.5,0.5";

        public static string NightVisionArgs
        {
            get { return nightVisionArgs; }
            set
            {
                nightVisionArgs = value;
                nightVisison = null;
            }
        }
        public static Material Get(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.Grayscale: return Grayscale;
                case ShaderType.NightVisison: return NightVisison;
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
        static Material nightVisison = null;
        public static Material NightVisison
        {
            get
            {
                if (nightVisison == null)
                {
                    nightVisison = new Material(
                        "Shader \"Nightvision\" {" +
                        "    Properties {" +
                        "        _MainTex (\"MainTex\", 2D) = \"white\" {}" +
                        "    }" +
                        "    SubShader {" +
                        "        Pass {" +
                        "            Blend SrcColor DstColor" +
                        "            SetTexture [_MainTex] {" +
                        "                constantColor (" + nightVisionArgs + ")" +
                        "                combine constant + texture" +
                        "            }" +
                        "        }" +
                        "        Pass {" +
                        "            Blend SrcColor DstColor" +
                        "            SetTexture [_MainTex] {" +
                        "                combine texture * previous" +
                        "            }" +
                        "        }" +
                        "    }" +
                        "} ");
                }
                return nightVisison;
            }
        }

    }
}
