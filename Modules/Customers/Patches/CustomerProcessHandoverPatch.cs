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
using Lithium.Modules.Customers.Architecture;
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

            List<ItemInstance> itemInstances = items.ToList();
            foreach (IBonusPaymentHandler handler in Core.Get<ModCustomers>().BonusPaymentHandlers)
            {
                if (handler.BonusPaymentHandler(__instance, contract, itemInstances, out List<Contract.BonusPayment> bonus) && bonus.Sum(b => b.Amount) > 0f)
                {
                    list.AddRange(bonus);
                }
            }

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
    }
}
