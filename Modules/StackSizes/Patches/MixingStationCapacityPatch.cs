using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace Lithium.Modules.StackSizes.Patches
{
    //[HarmonyPatch(typeof(MixingStation), nameof(MixingStation.Start))]
    public class MixingStationCapacityPatch
    {
        //[HarmonyPostfix]
        //public static void MixingRackCapacity(MixingStation __instance)
        //{
        //    ModStackSizesConfiguration config = Core.Get<ModStackSizes>().Configuration;
        //    if(!config.Enabled)
        //        return;

        //    __instance.MaxMixQuantity = config.MixingStation;
        //}
    }
}
