using HarmonyLib;
using Il2CppScheduleOne;

namespace Lithium.Modules.StackSizes.Patches
{
    [HarmonyPatch(typeof(Registry), nameof(Registry.Start))]
    public class RegistryStartPatch
    {
        [HarmonyPostfix]
        public static void RegistryStart(Registry __instance)
        {
            List<Registry.ItemRegister> itemRegistry = new List<Registry.ItemRegister>();
            foreach (Registry.ItemRegister register in __instance.ItemRegistry)
            {
                itemRegistry.Add(register);
            }

            ItemRegistry.AllItemDefinitions = itemRegistry
                .Select(itemRegister => itemRegister.Definition)
                .ToList();
                                    
            ItemRegistry.UpdateEntireRegistry();
        }
    }
}
