using Il2CppInterop.Runtime.Injection;
using Lithium.Modules.Customers.Behaviours;

namespace Lithium.Modules.Customers
{
    public class ModCustomersConfiguration : ModuleConfiguration
    {
        public override string Name => "Customers";
    }

    public class ModCustomers : ModuleBase<ModCustomersConfiguration>
    {
        public ModCustomers()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CustomerNotificationState>();
        }

        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
