using UnityEngine;

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

        static Styles()
        {
            Label13 = new GUIStyle { fontSize = 13, normal = new GUIStyleState { textColor = Color.white } };
            Label13B = new GUIStyle { fontSize = 13, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.white } };
            GreenLabel11 = new GUIStyle { fontSize = 11, normal = new GUIStyleState { textColor = Color.green } };
            GreenLabel13 = new GUIStyle { fontSize = 13, normal = new GUIStyleState { textColor = Color.green } };
            GreenLabel15B = new GUIStyle { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.green }, alignment = TextAnchor.MiddleCenter };
            RedLabel13 = new GUIStyle { fontSize = 13, normal = new GUIStyleState { textColor = Color.red } };
            RedLabel13B = new GUIStyle { fontSize = 13, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.red } };
            RedLabel25B = new GUIStyle { fontSize = 25, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.red }, alignment = TextAnchor.MiddleCenter };
        }
    }
}
