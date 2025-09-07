using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace Lithium.Modules.ChemistryStation.Patches
{
    [HarmonyPatch(typeof(ChemistryCookOperation), nameof(ChemistryCookOperation.Progress))]
    public class ChemistryCookOperationProgressPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ChemistryCookOperation __instance, int mins)
        {
            ModChemistryStationConfiguration config = Core.Get<ModChemistryStation>().Configuration;
            if (!config.Enabled)
                return;

            __instance.CurrentTime += config.BonusStepsPerTick;
        }
    }
}
