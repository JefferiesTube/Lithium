namespace Lithium.Modules.MixingStation
{
    public class ModMixingStationConfiguration : ModuleConfiguration
    {
        public override string Name => "MixingStation";
        public int InputCapacity { get; set; } = 20;
    }

    public class ModMixingStation : ModuleBase<ModMixingStationConfiguration>
    {
        public override void Apply()
        {
            if(!Configuration.Enabled)
                return;
        }
    }
}
