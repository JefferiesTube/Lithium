using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Quests;
using Lithium.Helper;
using Lithium.Modules.Customers.Behaviours;
using Lithium.Modules.TrashGrabber;
using Lithium.Util;
using UnityEngine;

namespace Lithium.Modules.Customers.Patches
{
    [HarmonyPatch(typeof(Customer), nameof(Customer.CheckContractGeneration))]
    public class CustomerContractGenerationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref ContractInfo __result, Customer __instance, bool force = false)
        {
            ModCustomersConfiguration config = Core.Get<ModCustomers>().Configuration;
            if (!config.Enabled)
                return;

            if (__result == null || force)
                return;

            if (LevelManager.Instance.TotalXP < 1400)
            {
                return;
            }
            
            List<string> desires = __instance.CustomerData.PreferredProperties
                .ToList()
                .Select(p => p.Name)
                .ToList();

            ProductList.Entry orderedProduct = __result.Products.entries.ToList()[0];
            string id = orderedProduct.ProductID;
            EQuality quality = orderedProduct.Quality;
            int quantity = orderedProduct.Quantity;

            if (__instance.AssignedDealer != null)
            {
                if (__instance.DealerHasSuitableProduct())
                {
                    List<(ProductItemInstance instance, int matchcount)> suitableItems = __instance.AssignedDealer.ItemSlots
                        .ToList()
                        .Where(i => i.ItemInstance is ProductItemInstance { Definition: ProductDefinition pd }
                                    && ProductHelper.ProductMatchesDesires(pd, desires))
                        .Select(i => (instance: i.ItemInstance as ProductItemInstance, 
                            matchcount: ProductHelper.GetMatchCount((i.ItemInstance as ProductItemInstance).Definition as ProductDefinition, desires)))
                        .ToList();

                    WeightedPicker<string> pickableItems = new();
                    foreach (var (instance, matchcount) in suitableItems)
                    {
                        if (instance == null || instance.Definition == null || instance.Quantity <= 0)
                            continue;

                        // TODO: Add special patterns here, once this module gets implemented
                        pickableItems.Add(instance.Definition.ID, matchcount);
                    }

                    id = pickableItems.Pick();
                    quantity = suitableItems
                        .FirstOrDefault(i => i.instance.Definition.ID == id).instance.Quantity;

                    RewireOrderedProduct(__result, id, quality,
                        Mathf.Clamp(quantity, 1, quantity));
                }
                else
                {
                    NotifyDealerNotSuitable(__instance);
                    __result = null;
                }
            }
            else
            {
                List<ProductDefinition> suitableProducts = ProductManager.ListedProducts
                    .ToList()
                    .Where(p => ProductHelper.ProductMatchesDesires(p, desires))
                    .OrderBy(x => UnityEngine.Random.value)
                    .ToList();

                if (suitableProducts.Count == 0)
                {
                    NotifyPlayerProductsNotSuitable(__instance);
                    __result = null;
                }
                else
                {
                    RewireOrderedProduct(__result, suitableProducts[0].ID, quality, quantity);
                }
            }
        }

        private static void RewireOrderedProduct(ContractInfo __result, string id, EQuality quality, int quantity)
        {
            ProductList list = new();
            list.entries.Add(new()
            {
                ProductID = id,
                Quality = quality,
                Quantity = quantity
            });
            __result.Products = list;
        }

        private static void NotifyPlayerProductsNotSuitable(Customer __instance)
        {
            if (__instance.TryGetComponent(out CustomerNotificationState state))
            {
                if (TimeManager.Instance.Playtime - state.LastNotification < 60 * 5)
                    return;
            }
            else
            {
                state = __instance.gameObject.AddComponent<CustomerNotificationState>();
                state.LastNotification = TimeManager.Instance.Playtime;
            }

            MessagingManager.Instance.ReceiveMessage(new(
                $"Hey, I wanted to get fresh stuff, but you don't offer good stuff. I prefer {ProductHelper.FormatDesires(__instance.CustomerData)}",
                Message.ESenderType.Other), true, __instance.NPC.ID);
            state.LastNotification = TimeManager.Instance.Playtime;
        }

        private static void NotifyDealerNotSuitable(Customer __instance)
        {
            if (__instance.TryGetComponent(out CustomerNotificationState state))
            {
                if (TimeManager.Instance.Playtime - state.LastNotification < 60 * 5)
                    return;
            }
            else
            {
                state = __instance.gameObject.AddComponent<CustomerNotificationState>();
                state.LastNotification = TimeManager.Instance.Playtime;
            }

            MessagingManager.Instance.ReceiveMessage(new(
                $"Hey, I wanted to get fresh stuff, but {__instance.AssignedDealer.FirstName} doesn't offer good stuff. I prefer {ProductHelper.FormatDesires(__instance.CustomerData)}",
                Message.ESenderType.Other), true, __instance.NPC.ID);
            state.LastNotification = TimeManager.Instance.Playtime;
        }
    }
}
