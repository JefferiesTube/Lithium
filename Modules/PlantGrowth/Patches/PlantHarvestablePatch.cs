using System.Globalization;
using Il2CppScheduleOne.Growing;
using HarmonyLib;

namespace Lithium.Modules.PlantGrowth.Patches
{
    [HarmonyPatch(typeof(PlantHarvestable), nameof(PlantHarvestable.Harvest))]
    public class PlantHarvestablePatch
    {
        [HarmonyPrefix]
        public static void Prefix(PlantHarvestable __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return;

            Plant componentInParent = __instance.GetComponentInParent<Plant>();
            __instance.name = componentInParent.QualityLevel.ToString(CultureInfo.InvariantCulture);
            componentInParent.QualityLevel += configuration.RandomYieldQualityPicker.Pick();
            __instance.ProductQuantity = (int) configuration.RandomYieldPerBudPicker.Pick();
        }

        [HarmonyPostfix]
        public static void Postfix(PlantHarvestable __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return;
            Plant componentInParent = __instance.GetComponentInParent<Plant>();
            if(float.TryParse(__instance.name, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out float result))
                componentInParent.QualityLevel = result;
        }
    }
}
