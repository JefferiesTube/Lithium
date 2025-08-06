using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace Lithium.Modules.LabOven.Patches
{
    [HarmonyPatch(typeof(OvenCookOperation), nameof(OvenCookOperation.IsReady))]
    public class OvenCookOperationIsReadyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(OvenCookOperation __instance, ref bool __result)
        {
            ModLabOvenConfiguration config = Core.Get<ModLabOven>().Configuration;
            if (!config.Enabled)
                return;

            __result = __instance.CookProgress >= __instance.GetCookDuration();
        }
    }
}
