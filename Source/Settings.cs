﻿using System;
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
        public bool _dist2500 = true;

        [GameParameters.CustomParameterUI("9999",
            toolTip = "Range of broadcast signal")]
        public bool _dist9999 = false;


        [GameParameters.CustomParameterUI("Scale camera capabilites to career mode",
            toolTip = "In career mode, cameras will initially have limited capabilities")]
        public bool scaleToCareer = false;


        [GameParameters.CustomParameterUI("Cam shutdown if out of range",
            toolTip = "Range of broadcast signal")]
        public bool FCS = false;

        [GameParameters.CustomParameterUI("Show cross",
            toolTip = "Cross lines which indicate info related to docking")]
        public bool showCross = true;

        [GameParameters.CustomParameterUI("Show summary data",
            toolTip = "Show Distance and closing Vel in upper-right of window")]
        public bool showSummaryData = true;

        [GameParameters.CustomParameterUI("Show data",
            toolTip = "Show docking data in upper-right of window")]
        public bool showData = true;

        [GameParameters.CustomParameterUI("Show rotator dials",
            toolTip = "Show dials in bottom of window")]
        public bool showDials = true;

        [GameParameters.CustomIntParameterUI("Default camera window size", minValue = 1, maxValue = 10,
            toolTip = "Value will be adjusted down at runtime if too large")]
        public int defaultCamWindowSize = 2;


        [GameParameters.CustomParameterUI("Use KSP skin")]
        public bool useKSPskin = false;

        [GameParameters.CustomParameterUI("Hide window(s) on F2")]
        public bool hideOnF2 = true;

        [GameParameters.CustomParameterUI("Use camera object for adjustments",
            toolTip = "Only used when using dev mode to adjust a camera's position")]
        public bool useCamObj = true;

        [GameParameters.CustomParameterUI("Use node object for adjustments",
            toolTip = "Only used when using dev mode to adjust a camera's position")]
        public bool useNodeObj = false;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

        }

        bool oDist2500;
        bool oDist9999;
        bool distInitted = false;


        bool oldUseCamObj, oldUseNodeObj;

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {

            if (oldUseCamObj == false && oldUseNodeObj == false)
            {
                oldUseCamObj = useCamObj;
                oldUseNodeObj = useNodeObj;
            }
            // The following is used to ensure that one and exactly one of the two
            // options are set

            if (useCamObj && !oldUseCamObj)
                useNodeObj = false;

            if (useNodeObj && !oldUseNodeObj)
                useCamObj = false;

            oldUseCamObj = useCamObj;
            oldUseNodeObj = useNodeObj;
            if (distInitted)
            {


                if (oDist2500 != _dist2500)
                {
                    _dist9999 = !_dist2500;
                }
                else if (oDist9999 != _dist9999)
                {
                    _dist2500 = !_dist9999;
                }
            }
            distInitted = true;
            oDist2500 = _dist2500;
            oDist9999 = _dist9999;

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
