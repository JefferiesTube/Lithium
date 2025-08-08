using HarmonyLib;
using Il2CppScheduleOne.Property;
using MelonLoader;
using UnityEngine;

namespace Lithium.Modules.Storyline.Patches
{
    [HarmonyPatch(typeof(RV), nameof(RV.SetExploded), [])]
    public class RVSetExplodedPatch
    {
        [HarmonyPrefix]
        public static bool DisableRVExplosion(RV __instance)
        {
            ModStorylineConfiguration config = Core.Get<ModStoryline>().Configuration;
            if (!config.Enabled || !config.PreventRVExplosion)
                return true;

            GameObject destroyedRV = GetChildGameObject(__instance.gameObject, "Destroyed RV");
            if (destroyedRV != null)
            {
                destroyedRV.SetActive(true);
                GameObject cartelNote = GetChildGameObject(destroyedRV, "CartelNote");
                if (cartelNote != null)
                {
                    cartelNote.SetActive(true);
                }

                GameObject destroyedRVChild = GetChildGameObject(destroyedRV, "destroyed rv");
                if (destroyedRVChild != null)
                {
                    destroyedRVChild.SetActive(false);
                }
            }
            else
            {
                MelonLogger.Msg("Destroyed RV not found");
            }

            return false;
        }


        private static GameObject GetChildGameObject(GameObject obj, string childName)
        {
            Transform transform = obj.transform.Find(childName);
            bool flag = transform != null;
            GameObject gameObject = flag ? transform.gameObject : null;
            return gameObject;
        }
    }
}