using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.NPCs.Behaviour;
using HarmonyLib;

namespace Lithium.Modules.PlantGrowth.Patches
{
    [HarmonyPatch(typeof(PotActionBehaviour), nameof(PotActionBehaviour.CompleteAction))]
    public class PotActionBehaviorPatch
    {
        public static void Postfix(PotActionBehaviour __instance)
        {
            ModPlantsConfiguration configuration = Core.Get<ModPlants>().Configuration;
            if (!configuration.Enabled)
                return;

            if (__instance.CurrentActionType != PotActionBehaviour.EActionType.Harvest) 
                return;
            if (__instance.botanist == null || __instance.botanist.Inventory == null) 
                return;

            ItemInstance item = __instance.botanist.MoveItemBehaviour.itemToRetrieveTemplate;
            if (item == null)
                return;
            item.Quantity = (int)Math.Min(item.Quantity * configuration.RandomYieldPerBudPicker.Pick(), item.StackLimit);
        }
    }
}
