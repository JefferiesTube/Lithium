namespace Lithium.Modules.LabOven
{
    public class ModLabOvenConfiguration : ModuleConfiguration
    {
        public override string Name => "LabOven";
        public float Speed = 1f;
    }
    public class ModLabOven : ModuleBase<ModLabOvenConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
