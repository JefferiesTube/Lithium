using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace Lithium.Modules.MixingStations.Patches
{
    [HarmonyPatch(typeof(MixingStation), nameof(MixingStation.Start))]
    public class MixingStationCapacityPatch
    {
        [HarmonyPostfix]
        public static void MixingStationCapacity(MixingStation __instance)
        {
            ModMixingStationsConfiguration config = Core.Get<ModMixingStations>().Configuration;
            if (!config.Enabled)
                return;

            __instance.MaxMixQuantity = config.InputCapacity;
        }
    }
}
