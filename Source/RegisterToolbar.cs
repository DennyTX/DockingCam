using System.Collections.Generic;
using KSP.IO;
using KSP.UI.Screens;
using OLDD_camera.Camera;
using OLDD_camera.Modules;
using OLDD_camera.Utils;
using ToolbarControl_NS;
using UnityEngine;
using UnityEngine.UI;

namespace OLDD_camera
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {

        void Start()
        {
            ToolbarControl.RegisterMod(DockCamToolbarButton.MODID, DockCamToolbarButton.MODNAME);          
        }
    }
}
