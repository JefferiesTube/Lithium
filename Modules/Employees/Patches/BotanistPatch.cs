using HarmonyLib;
using Il2CppScheduleOne.Employees;

namespace Lithium.Modules.Employees.Patches
{
    [HarmonyPatch(typeof(Botanist), nameof(Botanist.Start))]
    public class BotanistPatch
    {
        [HarmonyPrefix]
        static void Prefix(Botanist __instance)
        {
            ModEmployeesConfiguration config = Core.Get<ModEmployees>().Configuration;
            if (!config.Enabled)
                return;

            __instance.MaxAssignedPots = config.Botanists.MaxAssignedPots;
            __instance.configuration.AssignedStations.MaxItems = config.Botanists.MaxAssignedPots;

            __instance.Movement.WalkSpeed = config.Botanists.WalkSpeed;
            __instance.DailyWage = config.Botanists.DailyWage;
            __instance.SOIL_POUR_TIME = config.Botanists.SoilPourTime;
            __instance.WATER_POUR_TIME = config.Botanists.WaterPourTime;
            __instance.ADDITIVE_POUR_TIME = config.Botanists.AdditivePourTime;
            __instance.SEED_SOW_TIME = config.Botanists.SeedSowTime;
            __instance.HARVEST_TIME = config.Botanists.HarvestTime;
        }
    }
}
