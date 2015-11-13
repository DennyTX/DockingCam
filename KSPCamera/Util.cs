using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace DockingCamera
{
    /// <summary>
    /// Static class of utilities
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Standard path to the folder with the textures
        /// </summary>
        static string dataTexturePath = "OLDD/DockingCam/";
        /// <summary>
        /// Load Texture2D from standard folder
        /// </summary>
        /// <param name="name">Texture name without extension</param>
        /// <returns></returns>
        public static Texture2D LoadTexture(string name)
        {
            return GameDatabase.Instance.GetTexture(dataTexturePath + name, false);
        }

        /// <summary>
        /// Generate rectangle
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D MonoColorRectTexture(Color color)
        {
            return MonoColorTexture(color, 4, 4);
        }
        /// <summary>
        /// Generate vertical line
        /// </summary>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Texture2D MonoColorVerticalLineTexture(Color color, int size)
        {
            return MonoColorTexture(color, 1, size);
        }
        /// <summary>
        /// Generate horizontal line
        /// </summary>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Texture2D MonoColorHorizontalLineTexture(Color color, int size)
        {
            return MonoColorTexture(color, size, 1);
        }

        /// <summary>
        /// Texture generating a specific color
        /// </summary>
        /// <param name="color">Color of texture</param>
        /// <returns></returns>
        public static Texture2D MonoColorTexture(Color color, int width, int height)
        {
            var texture = new Texture2D(width, height);
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    texture.SetPixel(i, j, color);
            texture.Apply();
            return texture;
        }

        //whitenoise
        private static float alpha = .16f;  
        private static Color black = new Color(0, 0, 0, alpha);
        private static Color white = new Color(1, 1, 1, alpha);
        public static Texture2D WhiteNoiseTexture(int width, int height)
        {
            width *= 2;
            height *= 2;
            var texture = new Texture2D(width, height);
            var colors = new Color[width*height];
            for (int i = 0; i < width * height; i++)
            {
                colors[i] = UnityEngine.Random.Range(0, 100)%2 == 1 ? black : white;
            }
            texture.SetPixels(colors);
            texture.Apply();
            ///////////////////
            //object o = new object();
            //using (Stream s = new MemoryStream())
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(s, o);
            //    Debug.Log(new String('-', 10) + s.Length);
            //}
            /// /////////////////
            return texture;
        }
    }
}
