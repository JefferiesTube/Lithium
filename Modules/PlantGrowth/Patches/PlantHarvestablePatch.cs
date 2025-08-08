using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.PlayerScripts;
using Lithium.Modules.PlantGrowth.Behaviours;
using Object = UnityEngine.Object;

namespace Lithium.Modules.PlantGrowth.Patches
{
    [HarmonyPatch(typeof(PlantHarvestable), nameof(PlantHarvestable.Harvest))]
    public class PlantHarvestablePatch
    {
        private static readonly Dictionary<object, bool> SkipFlags = [];
        private static readonly Dictionary<object, bool> GenerateFlags = [];

        [HarmonyPrefix]
        public static bool Prefix(PlantHarvestable __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return true;

            Plant componentInParent = __instance.GetComponentInParent<Plant>();
            if (!componentInParent.TryGetComponent(out PlantBaseQuality comp))
            {
                PlantBaseQuality pbq = componentInParent.gameObject.AddComponent<PlantBaseQuality>();
                pbq.Quality = componentInParent.QualityLevel;
                pbq.NeedsNotification = true;
            }

            if (!GenerateFlags.ContainsKey(__instance))
            {
                componentInParent.QualityLevel += configuration.RandomYieldQualityPicker.Pick();
                __instance.ProductQuantity = (int)configuration.RandomYieldPerBudPicker.Pick();
                GenerateFlags[__instance] = true;
            }

            //if player cannot fit item ... skip harvest
            QualityItemInstance item = new QualityItemInstance(__instance.Product, __instance.ProductQuantity, ItemQuality.GetQuality(componentInParent.QualityLevel));
            if (!PlayerSingleton<PlayerInventory>.Instance.CanItemFitInInventory(item))
            {
                SkipFlags[__instance] = true;
                return false;
            }
            else
            {
                SkipFlags.Remove(__instance);
                return true;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(PlantHarvestable __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return;

            if (!SkipFlags.ContainsKey(__instance))
                return;

            GenerateFlags.Remove(__instance);

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
