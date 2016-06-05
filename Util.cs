using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;

namespace DockingCamera
{
    /// <summary>
    /// Static class of utilities
    /// </summary>
   // UPDATE_112 START
    public delegate void UpdateGUIFunction();
    public class UpdateGUIObject : MonoBehaviour
    {
        public UpdateGUIFunction updateGUIFunction;

        void OnGUI()
        {
            if (updateGUIFunction != null)
            {
                updateGUIFunction();
            }
        }
    }
    // UPDATE_112 END

    public static class Util
    {
        /// <summary>
        /// Standard path to the folder with the textures
        /// </summary>
        //static string timeMark; 
        static string dataTexturePath = "OLDD/DockingCam/";
        private static string PhotoDirectory = "Screenshots";
        
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
        public static Texture2D MonoColorHorizontalLineTexture(Color color, int size)
        {
            return MonoColorTexture(color, size, 1);
        }

        /// <summary>
        /// Texture generating a specific color
        /// </summary>
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
        public static Texture2D WhiteNoiseTexture(int width, int height, float alpha = .16f)
        {
            var black = new Color(0, 0, 0, alpha);
            var white = new Color(1, 1, 1, alpha);
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
            return texture;
        }

        /// <summary>
        /// This class converts time strings like "1d 2m 2s" into a
        /// double value as seconds and also vice versa, based on
        /// kerbin time.
        /// </summary>


        public static void SavePng(this RenderTexture renderTexture, string photoFrom)
        {
            var UniversalTime = Planetarium.fetch.time;
            //var MET = FlightGlobals.fetch.activeVessel.missionTime;
            var photoTime = GetTimeMark(UniversalTime);
            RenderTexture.active = renderTexture;
            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            var bytes = texture.EncodeToPNG();
            var name = string.Concat("Photo from ", photoFrom, " at UT ", photoTime, ".png");
            var folder = Path.Combine(PhotoDirectory, HighLogic.SaveFolder);
            Directory.CreateDirectory(folder);
            name = Path.Combine(folder, name);
            File.WriteAllBytes(name, bytes);
            ScreenMessages.PostScreenMessage("PHOTO HAS BEEN SAVED TO YOUR SCREENSHOTS FOLDER", 3f, ScreenMessageStyle.UPPER_CENTER);
        }
        public static String GetTimeMark(Double UniversalTime)
        {
            var time = UniversalTime;
            StringBuilder timeMark = new StringBuilder();
            if (time >= 9201600)
                time = converter(time, timeMark, 9201600, "y");
            if (time >= 21600)
                time = converter(time, timeMark, 21600, "d");
            if (time >= 3600)
                time = converter(time, timeMark, 3600, "h");
            if (time >= 60)
                time = converter(time, timeMark, 60, "m");
            timeMark.Append(time.ToString("F0"));
            timeMark.Append("s");
            return timeMark.ToString();
        }

        private static Double converter(Double time, StringBuilder timeMark, uint seconds, String suffix)
        {
            timeMark.Append(Math.Floor(time / seconds));
            timeMark.Append(suffix);
            return (time % seconds);
        }
    }
}
