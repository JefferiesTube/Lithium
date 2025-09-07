using Lithium.Modules.Customers;
using Lithium.Modules.EffectCombos.BonusPayments;

namespace Lithium.Modules.EffectCombos
{
    public class EffectCombo
    {
        public string Name { get; set; }
        public int FixedBonus { get; set; } = 0;
        public float PercentageBonus { get; set; } = 0f;
        public string[] Effects { get; set; } = [];
    }

    public class ModEffectCombosConfiguration : ModuleConfiguration
    {
        public override string Name => "EffectCombos";
        public bool AffectsDealers { get; set; } = true;
        public EffectCombo[] Combos { get; set; } = [];
    }
    public class ModEffectCombos : ModuleBase<ModEffectCombosConfiguration>
    {
        public override void Apply()
        {
            if(!Configuration.Enabled)
                return;

            Core.Get<ModCustomers>().RegisterBonusPaymentHandler(new EffectComboBonus());
        }
    }
}
