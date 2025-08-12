namespace Lithium.Modules.DryingRacks
{
    public class ModDryingRacksConfiguration : ModuleConfiguration
    {
        public override string Name => "DryingRacks";
        public int DryTimePerQuality = 720;
        public int Capacity { get; set; } = 20;
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
