namespace Lithium.Modules.DryingRacks
{
    public class ModDryingRacksConfiguration : ModuleConfiguration
    {
        public override string Name => "DryingRacks";
        public bool Enabled { get; set; } = false;

        public int DryTimePerQuality = 720;
    }

    public class ModDryingRacks : ModuleBase<ModDryingRacksConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
