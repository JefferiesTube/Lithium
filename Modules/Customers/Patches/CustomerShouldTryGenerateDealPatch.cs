//using HarmonyLib;
//using Il2CppScheduleOne.Economy;
//using Il2CppScheduleOne.Messaging;
//using Il2CppScheduleOne.NPCs;
//using Il2CppScheduleOne.Product;
//using Il2CppScheduleOne.Properties;
//using Lithium.Helper;

//namespace Lithium.Modules.Customers.Patches
//{
//    [HarmonyPatch(typeof(Customer), nameof(Customer.ShouldTryGenerateDeal))]

//    public class CustomerShouldTryGenerateDealPatch
//    {
//        [HarmonyPostfix]
//        static void Prefix(Customer __instance, ref bool __result)
//        {
//            if(!__result)
//                return;

//            List<Property> desires = __instance.CustomerData.PreferredProperties.ToList();

//            if (__instance.AssignedDealer == null)
//            {
//                if (DealerHasSuitableProduct(__instance.AssignedDealer, desires))
//                    return;

//                MessagingManager.Instance.ReceiveMessage(new("Hey, I wanted to get fresh stuff from your dealer, but there is no suitable offer", Message.ESenderType.Other), true, __instance.NPC.ID);
//                __result = false;
//                return;
//            }

//            List<ProductDefinition> suitableProducts = ProductManager.DiscoveredProducts.ToList()
//                .Where(p => p.Properties.ToList().Intersect(desires).Any())
//                .ToList();
//            if (suitableProducts.Count == 0 && desires.Count > 0)
//            {
//                MessagingManager.Instance.ReceiveMessage(new("Hey, I wanted to get fresh stuff, but you don't offer good stuff.", Message.ESenderType.Other), true, __instance.NPC.ID);
//                __result = false;
//                return;
//            }

//            __result = true;
//        }

//        private static bool DealerHasSuitableProduct(Dealer assignedDealer, List<Property> desires)
//        {
//            if(desires.Count == 0)
//                return true;

//            return assignedDealer.ItemSlots
//                .ToList()
//                .Any(i => i.ItemInstance is ProductItemInstance { Definition: ProductDefinition pd } 
//                                   && pd.Properties.ToList().Intersect(desires).Any());
//        }
//    }
//}