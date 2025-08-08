using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Lithium.Modules.PlantGrowth.Behaviours;

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

            PlantModified pm = __instance.gameObject.AddComponent<PlantModified>();
            pm.OriginalYieldLevel = __instance.YieldLevel;
            Plant plant = __instance.GetComponentInParent<Plant>();
            pm.QualityLevel = plant.QualityLevel;
            __instance.YieldLevel *= configuration.RandomYieldModifierPicker.Evaluate(UnityEngine.Random.value);
        }
    }
}
