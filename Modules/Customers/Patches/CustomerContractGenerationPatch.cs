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
            EQuality quality = orderedProduct.Quality;
            int quantity = orderedProduct.Quantity;

            if (__instance.AssignedDealer != null)
            {
                if (__instance.DealerHasSuitableProduct(out List<ItemInstance> dealerItems))
                {
                    List<ProductDefinition> products = dealerItems.Select(d => ProductManager.DiscoveredProducts.ToList().Single(p => p.ID.Equals(d.ID))).Distinct().ToList();
                    if(products.Count == 0)
                    {
                        NotifyDealerNotSuitable(__instance);
                        __result = null;
                        return;
                    }

                    Dictionary<string, int> productMaxQuantity = [];
                    foreach (ProductDefinition product in products)
                    {
                        if (product == null)
                            continue;
                        int maxQuantity = dealerItems
                            .Where(i => i.ID == product.ID)
                            .Sum(i => i.Quantity);
                        if (maxQuantity > 0)
                        {
                            productMaxQuantity[product.ID] = maxQuantity;
                        }
                    }

                    WeightedPicker<string> pickableItems = new();
                    foreach (KeyValuePair<string, int> entry in productMaxQuantity)
                    {
                        // TODO: Add special patterns here, once this module gets implemented
                        pickableItems.Add(entry.Key, ProductHelper.GetMatchCount(products.First(p => p.ID.Equals(entry.Key)).Properties.ToList(), desires));
                    }

                    string id = pickableItems.Pick();
                    quantity = productMaxQuantity[id];

                    RewireOrderedProduct(__result, id, quality, Mathf.Clamp(quantity, 1, quantity));
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

        private static void RewireOrderedProduct(ContractInfo __result, string id, EQuality quality, int maxAvailableQuantity)
        {
            int desiredQuantity = __result.Products.entries.ToList().Sum(e => e.Quantity);
            ProductList list = new();
            list.entries.Add(new()
            {
                ProductID = id,
                Quality = quality,
                Quantity = Mathf.Min(maxAvailableQuantity, desiredQuantity)
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
