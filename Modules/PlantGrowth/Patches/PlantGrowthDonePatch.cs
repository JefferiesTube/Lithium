using HarmonyLib;
using Il2CppScheduleOne.Growing;

namespace Lithium.Modules.PlantGrowth.Patches
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthDone))]
    public class PlantGrowthDonePatch
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return;

            if (__instance == null) 
                return;
            if (__instance.GetComponent<PlantModified>() != null) 
                return;

            __instance.gameObject.AddComponent<PlantModified>();
            __instance.YieldLevel *= configuration.RandomYieldModifierPicker.Pick();
            __instance.QualityLevel += configuration.RandomYieldQualityPicker.Pick();
        }
    }
}
