using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using UnityEngine;

namespace Lithium.Modules.MixingStations.Patches
{
    [HarmonyPatch(typeof(MixingStation), nameof(MixingStation.Start))]
    internal class MixingStationSpeedPatch
    {
        [HarmonyPostfix]
        public static void MixingStationSpeed(MixingStation __instance)
        {
            ModMixingStationsConfiguration config = Core.Get<ModMixingStations>().Configuration;
            if(!config.Enabled)
                return;

            __instance.MixTimePerItem *= Mathf.FloorToInt(1 / config.SpeedFactor);
        }
    }
}
