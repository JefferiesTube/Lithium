namespace Lithium.Modules.ChemistryStation
{
    public class ModChemistryStationConfiguration : ModuleConfiguration
    {
        public override string Name => "ChemistryStation";
        public int BonusStepsPerTick = 0;
    }
    public class ModChemistryStation : ModuleBase<ModChemistryStationConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
