using HarmonyLib;
using Il2CppScheduleOne.Employees;

namespace Lithium.Modules.Employees.Patches
{
    [HarmonyPatch(typeof(Chemist), nameof(Chemist.NetworkInitialize__Late))]
    public class ChemistValuesPatch
    {
        [HarmonyPostfix]
        static void Postfix(Chemist __instance)
        {
            if (!ModEmployees.ConfiguredEmployees.Add(__instance))
                return;

            ModEmployeesConfiguration config = Core.Get<ModEmployees>().Configuration;
            if (!config.Enabled)
                return;

            __instance.configuration.Stations.MaxItems = config.Chemists.MaxStations;
            __instance.Movement.WalkSpeed = config.Chemists.WalkSpeed;
            __instance.DailyWage = config.Chemists.DailyWage;
        }
    }
}
