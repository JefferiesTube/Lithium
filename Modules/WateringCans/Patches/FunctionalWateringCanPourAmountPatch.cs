using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace Lithium.Modules.WateringCans.Patches
{
    [HarmonyPatch(typeof(FunctionalWateringCan), nameof(FunctionalWateringCan.PourAmount))]
    public static class FunctionalWateringCanPourAmountPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref float amount)
        {
            ModWateringCanConfiguration configuration = Core.Get<ModWateringCan>().Configuration;

            if(!configuration.Enabled)
                return;

            amount *= configuration.DrainModifier;
        }
    }
}
