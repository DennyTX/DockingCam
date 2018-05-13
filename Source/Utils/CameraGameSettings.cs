using System.Collections;
using System.Reflection;

namespace OLDD_camera.Utils
{
   public class CameraGameSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Docking Camera"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Docking Camera"; } }
        public override string DisplaySection { get { return "Docking Camera"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Use Blizzy Toolbar", toolTip = "If available")]
        public bool useBlizzy = false;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }


        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
