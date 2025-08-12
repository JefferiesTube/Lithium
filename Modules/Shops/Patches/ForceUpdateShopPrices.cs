using HarmonyLib;
using Il2CppScheduleOne.UI.Shop;

namespace Lithium.Modules.Shops.Patches
{
    [HarmonyPatch(typeof(ShopInterface), nameof(ShopInterface.SetIsOpen))]
    public class ForceUpdateShopPrices
    {
        [HarmonyPostfix]
        public static void ShopInterfaceSetIsOpen(ShopInterface __instance, bool isOpen )
        {
            foreach (ListingUI ui in __instance.listingUI)
            {
                ui.UpdateLockStatus();
                ui.Update();
                ui.UpdatePrice();
                ui.UpdateStock();
                ui.UpdateButtons();
            }
        }
    }
}
