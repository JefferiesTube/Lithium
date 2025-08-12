using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.UI.Stations;

namespace Lithium.Modules.DryingRacks.Patches
{
    [HarmonyPatch(typeof(DryingRackCanvas), nameof(DryingRackCanvas.SetIsOpen))]
    public class DryingRackCapacityPatch
    {
        [HarmonyPostfix]
        public static void DryingRackCapacity(DryingRackCanvas __instance, DryingRack rack, bool open)
        {
            ModDryingRacksConfiguration config = Core.Get<ModDryingRacks>().Configuration;
            if(!config.Enabled)
                return;

            if (rack is not null)
            {
                rack.ItemCapacity = config.Capacity;
            }
        }
    }
}
