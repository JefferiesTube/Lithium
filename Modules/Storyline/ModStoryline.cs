namespace Lithium.Modules.Storyline
{
    public class ModStorylineConfiguration : ModuleConfiguration
    {
        public override string Name => "Storyline";

        public bool PreventRVExplosion { get; set; } = true;
    }

    public class ModStoryline : ModuleBase<ModStorylineConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
