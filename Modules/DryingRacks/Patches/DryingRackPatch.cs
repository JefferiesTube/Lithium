﻿using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;

namespace Lithium.Modules.DryingRacks.Patches
{
    [HarmonyPatch(typeof(DryingRack), nameof(DryingRack.MinPass))]
    public class DryingRackPatch
    {
        [HarmonyPrefix]
        static bool Prefix(DryingRack __instance)
        {
            if (!Core.Get<ModDryingRacks>().Configuration.Enabled)
                return true;

            int customThreshold = Core.Get<ModDryingRacks>().Configuration.DryTimePerQuality;

            // Clone the list to avoid modification issues during iteration
            foreach (DryingOperation dryingOperation in __instance.DryingOperations.ToArray())
            {
                dryingOperation.Time++;

                if (dryingOperation.Time >= customThreshold)
                {
                    if (dryingOperation.StartQuality >= EQuality.Premium)
                    {
                        if (InstanceFinder.IsServer &&
                            __instance.GetOutputCapacityForOperation(dryingOperation, EQuality.Heavenly) >= dryingOperation.Quantity)
                        {
                            __instance.TryEndOperation(
                                __instance.DryingOperations.IndexOf(dryingOperation),
                                false,
                                EQuality.Heavenly,
                                UnityEngine.Random.Range(int.MinValue, int.MaxValue)
                            );
                        }
                    }
                    else
                    {
                        dryingOperation.IncreaseQuality();
                    }
                }
            }

            return false; // Skip the original MinPass()
        }
    }
}
