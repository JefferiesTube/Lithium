namespace Lithium.Modules.WateringCans
{
    public class ModWateringCanConfiguration : ModuleConfiguration
    {
        public override string Name => "WateringCan";

        public float DrainModifier = 1.0f;
    }

    public class ModWateringCan : ModuleBase<ModWateringCanConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
