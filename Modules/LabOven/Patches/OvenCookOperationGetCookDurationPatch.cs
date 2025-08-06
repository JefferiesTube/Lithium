using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Lithium.Modules.Customers;
using UnityEngine;

namespace Lithium.Modules.LabOven.Patches
{
    [HarmonyPatch(typeof(OvenCookOperation), nameof(OvenCookOperation.GetCookDuration))]
    public class OvenCookOperationGetCookDurationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            ModLabOvenConfiguration config = Core.Get<ModLabOven>().Configuration;
            if (!config.Enabled)
                return;

            float speed = Core.Get<ModLabOven>().Configuration.Speed;
            __result = Mathf.FloorToInt(__result / speed);
        }
    }
}
