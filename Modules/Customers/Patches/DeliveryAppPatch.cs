using HarmonyLib;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.UI.Phone.Delivery;
using Lithium.Modules.Shops;
using Lithium.Helper;
using Lithium.Util;

namespace Lithium.Modules.Customers.Patches
{
    [HarmonyPatch(typeof(DeliveryApp), nameof(DeliveryApp.SetOpen))]
    public class DeliveryAppPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool open, DeliveryApp __instance)
        {
            ModShops modShops = Core.Get<ModShops>();
            if (modShops == null || !modShops.Configuration.Enabled)
                return;

            ModShopsConfiguration config = Core.Get<ModShops>().Configuration;

            if (config.Deliveries == null)
            {
                config.Deliveries = DeliveryApp.Instance.deliveryShops.ToList()
                    .ToDictionary(d => d.MatchingShop.ShopName,
                        d => new DeliverySettings
                        {
                            Availability = DeliveryAvailabilitySettings.Unchanged,
                            DeliveryFee = d.DeliveryFee,
                            XPRequirement = 0
                        });
                config.SaveConfiguration();
            }

            DeliveryUtils.ApplyDeliveryOverrides();
        }
    }
}
