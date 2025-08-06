using HarmonyLib;
using Il2CppScheduleOne.Equipping;

namespace Lithium.Modules.TrashGrabber.Patches
{
    [HarmonyPatch(typeof(Equippable_TrashGrabber), nameof(Equippable_TrashGrabber.GetCapacity))]
    public static class EquippableTrashGrabberGetCapacityPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, Equippable_TrashGrabber __instance)
        {
            ModTrashGrabberConfiguration config = Core.Get<ModTrashGrabber>().Configuration;
            if (!config.Enabled)
                return;
            
            __result =  config.CustomCapacity - __instance.trashGrabberInstance.GetTotalSize();
        }
    }
}
