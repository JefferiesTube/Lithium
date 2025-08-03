namespace Lithium.Modules.DryingRacks
{
    public class ModCustomersConfiguration : ModuleConfiguration
    {
        public override string Name => "Customers";
        public bool Enabled { get; set; } = false;
    }

    public class ModCustomers : ModuleBase<ModCustomersConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
