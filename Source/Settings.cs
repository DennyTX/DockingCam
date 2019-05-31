using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace OLDD_camera
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    // HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().
    public class KURSSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "General"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Docking Camera KURS Style"; } }
        public override string DisplaySection { get { return "Docking Camera KURS Style"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("2250",
            toolTip = "Range of broadcast signal")]
        public bool _dist2500 = false;

        [GameParameters.CustomParameterUI("9999",
            toolTip = "Range of broadcast signal")]
        public bool _dist9999 = false;

        [GameParameters.CustomParameterUI("Scale camera capabilites to career mode",
            toolTip = "In career mode, cameras will initially have limited capabilities")]
        public bool scaleToCareer = false;


        [GameParameters.CustomParameterUI("Cam shutdown if out of range",
            toolTip = "Range of broadcast signal")]
        public bool FCS = false;


        [GameParameters.CustomParameterUI("Use KSP skin")]
        public bool useKSPskin = false;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            // if (member.Name == "DefaultSettings" && DefaultSettings)
            //SetDifficultyPreset(parameters.preset);

            if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Parameters != null)
            {
                if (OLDD_camera.Utils.Styles.KspSkin != HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().useKSPskin)
                {
                    OLDD_camera.Utils.Styles.InitStyles();
                }
            }
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
