#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OLDD_camera.Utils
{
    // only start once at the MainMenu
    //[KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KSPShader : MonoBehaviour
    {

        private Dictionary<string, Shader> allShaders = new Dictionary<string, Shader>();


        /// <summary>
        /// Initial Unity Awake call
        /// </summary>
        public void Awake()
        {
            LoadShaders();

            var myshader = GetShader("ShaderNG/TypeC");

            if (myshader == null)
            {
                Debug.Log("Shader ShaderNG/TypeC was not loaded");
            }
            else
            {
                Debug.Log("Shader found");
            }

        }

        internal void LoadShaders()
        {
            // here we force unity to load all shaders, even those that are not used by any material, so we can find ours.
            Shader.WarmupAllShaders();
            foreach (var shader in Resources.FindObjectsOfTypeAll<Shader>())
            {
                if (!allShaders.ContainsKey(shader.name))
                {
                    Debug.Log("ShaderLoaderTest, adding: " + shader.name);
                    allShaders.Add(shader.name, shader);
                }
            }
        }

        // replacement function for Shader.find(). will return any valid shader in the System.
        internal Shader GetShader(string name)
        {
            if (allShaders.ContainsKey(name))
            {
                return allShaders[name];
            }
            else
            {
                return null;
            }
        }
    }
}
#endif