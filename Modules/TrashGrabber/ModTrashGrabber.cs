namespace Lithium.Modules.TrashGrabber
{
    public class ModTrashGrabberConfiguration : ModuleConfiguration
    {
        public override string Name => "TrashGrabber";

        public int CustomCapacity { get; set; } = 20;
    }
    public class ModTrashGrabber : ModuleBase<ModTrashGrabberConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
