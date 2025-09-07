using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Quests;
using Lithium.Modules.Customers.Architecture;
using MelonLoader;
using UnityEngine;

namespace Lithium.Modules.Customers.BonusPayments
{
    public class QualityBonus : IBonusPaymentHandler
    {
        public bool BonusPaymentHandler(Customer customer, Contract contract, List<ItemInstance> items, out List<Contract.BonusPayment> boni)
        {
            boni = [];
            ModCustomersConfiguration config = Core.Get<ModCustomers>().Configuration;

            if (!config.QualityBonus.Enabled)
                return false;

            if (!config.QualityBonus.AffectsDealers && contract.Dealer != null)
                return false;

            float qualityScalar = CustomerData.GetQualityScalar(customer.customerData.Standards.GetCorrespondingQuality());
            float minQuality = 1f;
            foreach (ItemInstance itemInstance in items)
            {
                ProductItemInstance productItemInstance = itemInstance.TryCast<ProductItemInstance>();
                if (productItemInstance != null)
                {
                    float quality = CustomerData.GetQualityScalar(productItemInstance.Quality);
                    if (quality < minQuality)
                    {
                        minQuality = quality;
                    }
                }
                else
                {
                    MelonLogger.Msg("Found null product item! Please report this bug.");
                }
            }

            float qualityDiff = minQuality - qualityScalar;
            if (qualityDiff > 0)
            {
                int bonusLevels = Mathf.RoundToInt(qualityDiff / 0.25f);
                float bonusAmount = contract.Payment * (bonusLevels * config.QualityBonus.BonusPercentage / 100f);
                boni.Add(new("Quality Bonus", bonusAmount));
                return true;
            }

            return false;
        }
    }
}
