using UnityEngine;
using UnityEngine.UI;

namespace OLDD_camera.Utils
{
    public static class Styles
    {
        public static GUIStyle Label13 { get; set; }
        public static GUIStyle Label13B { get; set; }
        public static GUIStyle GreenLabel11 { get; set; }
        public static GUIStyle GreenLabel13 { get; set; }
        public static GUIStyle GreenLabel15B { get; set; }
        public static GUIStyle RedLabel13 { get; set; }
        public static GUIStyle RedLabel13B { get; set; }
        public static GUIStyle RedLabel25B { get; set; }

        const int KSP_SKIN_ADJUST = 4;

        public static bool Initted = false;
        public static bool KspSkin = false;
        static Styles()
        {
            InitStyles();
            Initted = true;
        }
        internal static void InitStyles()
        { 
            Label13 = new GUIStyle { fontSize = 13, normal = new GUIStyleState { textColor = Color.white } };
            Label13B = new GUIStyle { fontSize = 13, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.white } };
            GreenLabel11 = new GUIStyle { fontSize = 11, normal = new GUIStyleState { textColor = Color.green } };
            GreenLabel13 = new GUIStyle { fontSize = 13, normal = new GUIStyleState { textColor = Color.green } };
            GreenLabel15B = new GUIStyle { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.green }, alignment = TextAnchor.MiddleLeft };
            RedLabel13 = new GUIStyle { fontSize = 13, normal = new GUIStyleState { textColor = Color.red } };
            RedLabel13B = new GUIStyle { fontSize = 13, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.red } };
            RedLabel25B = new GUIStyle { fontSize = 25, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.red }, alignment = TextAnchor.MiddleCenter };
            if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Parameters != null)
            {
                KspSkin = HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().useKSPskin;
                if (HighLogic.CurrentGame.Parameters.CustomParams<KURSSettings>().useKSPskin)
                {
                    Label13.fontSize += KSP_SKIN_ADJUST;
                    Label13B.fontSize += KSP_SKIN_ADJUST;
                    GreenLabel11.fontSize += KSP_SKIN_ADJUST;
                    GreenLabel13.fontSize += KSP_SKIN_ADJUST;
                    GreenLabel15B.fontSize += KSP_SKIN_ADJUST;
                    RedLabel13.fontSize += KSP_SKIN_ADJUST;
                    RedLabel13B.fontSize += KSP_SKIN_ADJUST;
                    RedLabel25B.fontSize += KSP_SKIN_ADJUST;
                }
            }
        }
    }
}
