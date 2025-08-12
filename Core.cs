using Il2CppScheduleOne.Growing;
using Lithium.Modules;
using Lithium.Modules.Customers;
using Lithium.Modules.DryingRacks;
using Lithium.Modules.LabOven;
using Lithium.Modules.MixingStations;
using Lithium.Modules.PlantGrowth;
using Lithium.Modules.PropertyPrices;
using Lithium.Modules.Shops;
using Lithium.Modules.StackSizes;
using Lithium.Modules.Storyline;
using Lithium.Modules.TrashGrabber;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Lithium.Core), "Lithium", "1.0.0", "DerTomDer & YukiSora", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Lithium
{
    public class Core : MelonMod
    {
        public static readonly List<ModuleBase> Modules = 
        [
            new ModPropertyPrices(),
            new ModPlants(),
            new ModDryingRacks(),
            new ModCustomers(),
            new ModStackSizes(),
            new ModLabOven(),
            new ModTrashGrabber(),
            new ModMixingStations(),
            new ModStoryline(),
            new ModShops(),
        ];

        public static T Get<T>() where T : ModuleBase => Modules.OfType<T>().FirstOrDefault();

        //public override void OnUpdate()
        //{
        //    base.OnUpdate();
        //    if (Input.GetKeyDown(KeyCode.F5))
        //    {
        //        var x = UnityEngine.Object.FindObjectsOfType<Additive>().ToList();
        //    }
        //}

        public override void OnInitializeMelon()
        {
            foreach (ModuleBase module in Modules)
            {
                LoggerInstance.Msg($"Loading {module.GetType().Name}");
                module.Load();
            }

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("com.lithium");
            harmony.PatchAll();

            LoggerInstance.Msg("Lithium initialized");
        }

        private bool _isFirstStart = true;
        
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                foreach (ModuleBase module in Modules)
                {
                    LoggerInstance.Msg($"Loading {module.GetType().Name}");
                    module.Apply();
                }
                
                _isFirstStart = false;
            }
            else if (sceneName.Equals("Menu", StringComparison.OrdinalIgnoreCase) && !_isFirstStart)
            {
                _isFirstStart = true;
            }
        }


        //public static Sprite FindSprite(string spriteName)
        //{
        //    try
        //    {
        //        foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
        //        {
        //            if (sprite.name == spriteName)
        //            {
        //                Logger.Debug($"Found sprite '{spriteName}' directly in loaded objects");
        //                return sprite;
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error($"Error finding sprite '{spriteName}': {ex.Message}");
        //        return null;
        //    }
        //}
    }
}
