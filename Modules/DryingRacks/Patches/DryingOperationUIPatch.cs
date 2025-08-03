﻿using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.UI.Stations.Drying_rack;
using UnityEngine;

namespace Lithium.Modules.DryingRacks.Patches
{
    [HarmonyPatch(typeof(DryingOperationUI), nameof(DryingOperationUI.UpdatePosition))]
    public class DryingOperationUIPatch
    {
        [HarmonyPrefix]
        static bool Prefix(DryingOperationUI __instance)
        {
            if (!Core.Get<ModDryingRacks>().Configuration.Enabled)
                return true;

            int configValue = Core.Get<ModDryingRacks>().Configuration.DryTimePerQuality;
            DryingOperation op = __instance.AssignedOperation;

            float tNorm = Mathf.Clamp01((float)op.Time / configValue);
            int timeLeft = Mathf.Clamp(configValue - op.Time, 0, configValue);
            int hours = timeLeft / 60;
            int minutes = timeLeft % 60;

            __instance.Tooltip.text = $"{hours}h {minutes}m until next tier";

            float left = -62.5f;
            float right = -left;
            __instance.Rect.anchoredPosition = new Vector2(Mathf.Lerp(left, right, tNorm), 0f);

            return false; // Skip the original method
        }
    }
}
