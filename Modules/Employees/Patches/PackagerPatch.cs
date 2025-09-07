using HarmonyLib;
using Il2CppScheduleOne.Employees;

namespace Lithium.Modules.Employees.Patches
{
    [HarmonyPatch(typeof(Packager), nameof(Packager.NetworkInitialize__Late))]
    public class PackagerPatch
    {
        [HarmonyPostfix]
        static void Postfix(Packager __instance)
        {
            if (!ModEmployees.ConfiguredEmployees.Add(__instance))
                return;

            ModEmployeesConfiguration config = Core.Get<ModEmployees>().Configuration;
            if (!config.Enabled)
                return;

            __instance.configuration.Stations.MaxItems = config.Packagers.MaxStations;
            __instance.configuration.Routes.MaxRoutes = config.Packagers.MaxRoutes;
            __instance.Movement.WalkSpeed = config.Packagers.WalkSpeed;
            __instance.DailyWage = config.Packagers.DailyWage;
            __instance.PackagingSpeedMultiplier = config.Packagers.PackagingSpeedMultiplier;
        }
    }
}
