using Il2CppScheduleOne.ItemFramework;
using HarmonyLib;
using Il2CppScheduleOne;

namespace Lithium.Modules.StackSizes.Patches
{
    [HarmonyPatch(typeof(Registry), nameof(Registry.AddToRegistry))]
    public class RegistryAddToRegistryPatch
    {
        [HarmonyPostfix]
        public static void RegistryAddToRegistry(Registry __instance, ItemDefinition item)
        {
            ItemRegistry.AllItemDefinitions.AddItem(item);
            ItemRegistry.UpdateItemDefinition(item);
        }
    }
}
