using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.UI;
using System.Globalization;
using Il2CppScheduleOne.ItemFramework;
using Lithium.Modules.PlantGrowth.Behaviours;
using UnityEngine;
using Object = UnityEngine.Object;

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
            if (!componentInParent.TryGetComponent(out PlantBaseQuality comp))
            {
                PlantBaseQuality pbq = componentInParent.gameObject.AddComponent<PlantBaseQuality>();
                pbq.Quality = componentInParent.QualityLevel;
                pbq.NeedsNotification = true;
            }

            componentInParent.QualityLevel +=  configuration.RandomYieldQualityPicker.Pick();
            __instance.ProductQuantity = (int) configuration.RandomYieldPerBudPicker.Pick();
        }

        [HarmonyPostfix]
        public static void Postfix(PlantHarvestable __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return;
            Plant componentInParent = __instance.GetComponentInParent<Plant>();
            if (componentInParent.TryGetComponent(out PlantBaseQuality comp) && comp.NeedsNotification)
            {
                EQuality quality = ItemQuality.GetQuality(componentInParent.QualityLevel);

                NotificationsManager.Instance.SendNotification($"{__instance.ProductQuantity}x {componentInParent.SeedDefinition.Name}",
                    $"{quality:G} quality", componentInParent.SeedDefinition.Icon, 2f, false);
                componentInParent.QualityLevel = comp.Quality;
                comp.NeedsNotification = false;
                Object.Destroy(comp);
            }
        }
    }
}
