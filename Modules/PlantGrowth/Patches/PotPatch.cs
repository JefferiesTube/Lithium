using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Lithium.Modules.PlantGrowth.Behaviours;
using MelonLoader;
using UnityEngine;

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
            float growthModifier = Core.Get<ModPlants>().Configuration.GrowthModifier;

            if (growthModifier <= 0.001f)
            {
                MelonLogger.Error("Invalid Growth Modifier. Skipping patch");
                return;
            }

            __result *= Mathf.Max(0.001f, growthModifier);
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
            if (__instance == null)
                return;

            PotBaseValues potBaseValues = __instance.gameObject.GetComponent<PotBaseValues>();
            if (potBaseValues == null)
                return;

            float baseDrain = potBaseValues.BaseWaterDrainPerHour;
            __instance.WaterDrainPerHour = baseDrain * Core.Get<ModPlants>().Configuration.WaterDrainModifier;
        }
    }
}
