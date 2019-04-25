namespace OLDD_camera.Utils
{
    public class ModuleSpyExperiment : ModuleScienceExperiment
    {
        [KSPEvent(guiName = "Deploy", active = true, guiActive = false)]
        public new void DeployExperiment()
        {
            base.DeployExperiment();
        }

        [KSPEvent(guiName = "Review Data", active = true, guiActive = true)]
        public new void ReviewDataEvent()
        {
            base.ReviewData();
        }
    }
}