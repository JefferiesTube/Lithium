using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Variables;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace Lithium.Modules.MixingStations.Patches
{
    [HarmonyPatch(typeof(MixingStation), nameof(MixingStation.MinPass))]
    internal class MixingStationSpeedPatch
    {
        [HarmonyPrefix]
        public static bool MixingStationSpeed(MixingStation __instance)
        {
            ModMixingStationsConfiguration config = Core.Get<ModMixingStations>().Configuration;
            if(!config.Enabled)
                return true;

            if (__instance.CurrentMixOperation != null || __instance.OutputSlot.Quantity > 0)
            {
                int num = 0;
                if (__instance.CurrentMixOperation != null)
                {
                    int currentMixTime = __instance.CurrentMixTime;
                    int currentMixTime2 = __instance.CurrentMixTime;
                    __instance.CurrentMixTime = currentMixTime2 + config.MixStepsPerSecond;
                    num = __instance.GetMixTimeForCurrentOperation();
                    if (__instance.CurrentMixTime >= num && currentMixTime < num && InstanceFinder.IsServer)
                    {
                        NetworkSingleton<VariableDatabase>.Instance.SetVariableValue("Mixing_Operations_Completed", (NetworkSingleton<VariableDatabase>.Instance.GetValue<float>("Mixing_Operations_Completed") + 1f).ToString(), true);
                        __instance.MixingDone_Networked();
                    }
                }
                if (__instance.Clock != null)
                {
                    __instance.Clock.SetScreenLit(true);
                    __instance.Clock.DisplayMinutes(Mathf.Max(num - __instance.CurrentMixTime, 0));
                }
                if (__instance.Light != null)
                {
                    if (__instance.IsMixingDone)
                    {
                        __instance.Light.isOn = NetworkSingleton<TimeManager>.Instance.DailyMinTotal % 2 == 0;
                        return false;
                    }
                    __instance.Light.isOn = true;
                    return false;
                }
            }
            else
            {
                if (__instance.Clock != null)
                {
                    __instance.Clock.SetScreenLit(false);
                    __instance.Clock.DisplayText(string.Empty);
                }
                if (__instance.Light != null && __instance.IsMixingDone)
                {
                    __instance.Light.isOn = false;
                }
            }

            return false;
        }
    }
}
