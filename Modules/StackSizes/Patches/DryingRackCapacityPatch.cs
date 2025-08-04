using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.UI.Stations;

namespace Lithium.Modules.StackSizes.Patches
{
    [HarmonyPatch(typeof(DryingRackCanvas), nameof(DryingRackCanvas.SetIsOpen))]
    public class DryingRackCapacityPatch
    {
        [HarmonyPostfix]
        public static void DryingRackCapacity(DryingRackCanvas __instance, DryingRack rack, bool open)
        {
            ModStackSizesConfiguration config = Core.Get<ModStackSizes>().Configuration;
            if(!config.Enabled)
                return;

            if (rack is not null)
            {
                rack.ItemCapacity = config.DryingRack;
            }
        }
    }
}
