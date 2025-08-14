using Il2CppScheduleOne.ItemFramework;

namespace Lithium.Modules.DryingRacks
{
    public class ModDryingRacksConfiguration : ModuleConfiguration
    {
        public override string Name => "DryingRacks";
        public int Capacity { get; set; } = 20;
        public Dictionary<string, int> PerQualityDryTimes = new()
        {
            { nameof(EQuality.Trash), 720 },
            { nameof(EQuality.Poor), 720 },
            { nameof(EQuality.Standard), 720 },
            { nameof(EQuality.Premium), 720 },
            { nameof(EQuality.Heavenly), 720 },
        };
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
