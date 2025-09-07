using HarmonyLib;
using Il2CppFishNet.Object;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Law;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Handover;
using Lithium.Helper;
using MelonLoader;
using UnityEngine;

namespace Lithium.Modules.Customers.Patches
{
    [HarmonyPatch(typeof(Customer), nameof(Customer.ProcessHandover))]
    public static class CustomerProcessHandoverPatch
    {
        public static bool Prefix(
            Customer __instance,
            HandoverScreen.EHandoverOutcome outcome,
            Contract contract,
            Il2CppSystem.Collections.Generic.List<ItemInstance> items,
            bool handoverByPlayer,
            bool giveBonuses)
        {
            ModCustomersConfiguration config = Core.Get<ModCustomers>().Configuration;
            if (!config.Enabled || !config.EffectBonus.Enabled)
                return true;

            if (!handoverByPlayer && !config.EffectBonus.AffectsDealers)
                return true;

            float satisfaction = Mathf.Clamp01(__instance.EvaluateDelivery(contract, items, out float highestAddiction, out EDrugType mainDrugType, out int matchedProductCount));
            __instance.ChangeAddiction(satisfaction / 5f);

            float relationDelta = __instance.NPC.RelationData.RelationDelta;
            float relationshipChange = CustomerSatisfaction.GetRelationshipChange(satisfaction);
            float affinityChange = relationshipChange * 0.2f * Mathf.Lerp(0.75f, 1.5f, highestAddiction);
            __instance.AdjustAffinity(mainDrugType, affinityChange);
            __instance.NPC.RelationData.ChangeRelationship(relationshipChange, true);
            List<Contract.BonusPayment> list = new List<Contract.BonusPayment>();

            if (NetworkSingleton<CurfewManager>.Instance.IsCurrentlyActive)
            {
                list.Add(new("Curfew Bonus", contract.Payment * 0.2f));
            }
            if (matchedProductCount > contract.ProductList.GetTotalQuantity())
            {
                list.Add(new("Generosity Bonus", 10f * (matchedProductCount - contract.ProductList.GetTotalQuantity())));
            }
            GameDateTime acceptTime = contract.AcceptTime;
            GameDateTime gameDateTime = new GameDateTime(acceptTime.elapsedDays, TimeManager.AddMinutesTo24HourTime(contract.DeliveryWindow.WindowStartTime, 60));
            if (NetworkSingleton<TimeManager>.Instance.IsCurrentDateWithinRange(acceptTime, gameDateTime))
            {
                list.Add(new("Quick Delivery Bonus", contract.Payment * 0.1f));
            }

            if (config.QualityBonus.Enabled)
                ApplyQualityBonus(__instance, contract, items, config, list);

            if(config.EffectBonus.Enabled)
                ApplyEffectBonus(__instance, contract, items, config, list);

            float totalBonus = 0f;
            foreach (Contract.BonusPayment bonusPayment in list)
            {
                MelonLogger.Msg($"Bonus: {bonusPayment.Title} Amount: {bonusPayment.Amount}");
                totalBonus += bonusPayment.Amount;
            }
            if (handoverByPlayer)
            {
                Singleton<HandoverScreen>.Instance.ClearCustomerSlots(false);
                contract.SubmitPayment(totalBonus);
            }
            if (outcome == HandoverScreen.EHandoverOutcome.Finalize && handoverByPlayer)
            {
                Singleton<DealCompletionPopup>.Instance.PlayPopup(__instance, satisfaction, relationDelta, contract.Payment, list.ToIL2CPPList());
            }

            __instance.TimeSinceLastDealCompleted = 0;
            __instance.NPC.SendAnimationTrigger("GrabItem");
            NetworkObject networkObject = null;
            if (contract.Dealer != null)
            {
                networkObject = contract.Dealer.NetworkObject;
            }

            MelonLogger.Msg($"Base payment: {contract.Payment} Total bonus: {totalBonus} Satisfaction: {satisfaction} Dealer: {networkObject?.name ?? "none"}");
            float clampedValue = Mathf.Clamp(contract.Payment + totalBonus, 0f, float.MaxValue);
            __instance.ProcessHandoverServerSide(outcome, items, handoverByPlayer, clampedValue, contract.ProductList, satisfaction, networkObject);


            return false;
        }

        private static void ApplyEffectBonus(Customer customer, Contract contract, Il2CppSystem.Collections.Generic.List<ItemInstance> items,
            ModCustomersConfiguration config, List<Contract.BonusPayment> list)
        {
            List<string> desires = customer.CustomerData.PreferredProperties.ToList().Select(p => p.Name.ToLowerInvariant()).ToList();
            if (desires.Count == 0 || items == null || items.Count == 0)
                return;

            int totalUnits = items.ToList().Sum(i => i?.Quantity ?? 0);

            float totalBonusAmount = 0f;
            
            foreach (ItemInstance item in items)
            {
                if (item == null) 
                    continue;


                ProductDefinition pd = ProductManager.DiscoveredProducts.ToList().SingleOrDefault(p => p.ID.Equals(item.ID));
                if(pd == null)
                    continue;

                int matchCount = pd.Properties.ToList().Select(p => p.Name.ToLowerInvariant()).Intersect(desires).Count();
                if(matchCount <= 0)
                    continue;

                EffectMatchBonus bucket = matchCount switch
                {
                    1 => config.EffectBonus.OneCoveredEffect,
                    2 => config.EffectBonus.TwoCoveredEffects,
                    _ => config.EffectBonus.ThreeCoveredEffects
                };

                int quantity = Mathf.Max(1, item.Quantity);
                int fixedMin = Mathf.Min(bucket.FixedBonusMin, bucket.FixedBonusMax);
                int fixedMax = Mathf.Max(bucket.FixedBonusMin, bucket.FixedBonusMax);
                float pctMin = Mathf.Min(bucket.PercentageBonusMin, bucket.PercentageBonusMax);
                float pctMax = Mathf.Max(bucket.PercentageBonusMin, bucket.PercentageBonusMax);
                int fixedPerUnit = (fixedMax > fixedMin) ? UnityEngine.Random.Range(fixedMin, fixedMax + 1) : fixedMin;
                float pctForThisItem = (pctMax > pctMin) ? UnityEngine.Random.Range(pctMin, pctMax) : pctMin;

                float fixedPart = fixedPerUnit * quantity;
                float deliveryShare = quantity / (float)totalUnits; // in [0,1]
                float percentPart = contract.Payment * (pctForThisItem / 100f) * deliveryShare;

                float itemBonus = fixedPart + percentPart;
                totalBonusAmount += itemBonus;
                MelonLogger.Msg($"Effect match bonus for item {pd.Name}: x{quantity} (matches: {matchCount}) Fixed: {fixedPart} Percent: {percentPart} Total: {itemBonus}");
            }

            if (totalBonusAmount <= 0f) 
                return;

            list.Add(new("Effect Match Bonus", totalBonusAmount));
        }

        private static void ApplyQualityBonus(Customer __instance, Contract contract, Il2CppSystem.Collections.Generic.List<ItemInstance> items,
            ModCustomersConfiguration config, List<Contract.BonusPayment> list)
        {
            float qualityScalar = CustomerData.GetQualityScalar(__instance.customerData.Standards.GetCorrespondingQuality());
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
                list.Add(new("Quality Bonus", bonusAmount));
            }
        }
    }
}
