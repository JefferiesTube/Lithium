using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Lithium.Modules.PlantGrowth.Behaviours;

namespace Lithium.Modules.PlantGrowth.Patches
{
    [HarmonyPatch(typeof(Pot), nameof(Pot.GetAdditiveGrowthMultiplier))]
    public class PotPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            if (!Core.Get<ModPlants>().Configuration.Enabled)
                return;
            __result *= Core.Get<ModPlants>().Configuration.GrowthModifier;
        }
    }

    [HarmonyPatch(typeof(Pot), nameof(Pot.Start))]
    public class PotStartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pot __instance)
        {
            if (!Core.Get<ModPlants>().Configuration.Enabled)
                return;
            __instance.gameObject.AddComponent<PotBaseValues>().Init(__instance);
        }
    }

    [HarmonyPatch(typeof(Pot), nameof(Pot.OnMinPass))]
    public class PotMinPassPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Pot __instance)
        {
            if (!Core.Get<ModPlants>().Configuration.Enabled)
                return;

            PotBaseValues potBaseValues = __instance.gameObject.GetComponent<PotBaseValues>();
            if (potBaseValues == null)
                return;

            float baseDrain = __instance.gameObject.GetComponent<PotBaseValues>().BaseWaterDrainPerHour;
            __instance.WaterDrainPerHour = baseDrain * Core.Get<ModPlants>().Configuration.WaterDrainModifier;
        }
    }
}
