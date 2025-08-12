using Il2CppScheduleOne.UI.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppScheduleOne.Economy;

namespace Lithium.Modules.Shops.Patches
{
    [HarmonyPatch(typeof(Supplier), nameof(Supplier.Start))]
    public class SupplierStartPatch
    {
        [HarmonyPrefix]
        public static void PatchPrices(Supplier __instance)
        {
                
        }
    }
}
