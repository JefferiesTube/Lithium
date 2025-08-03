using Il2CppScheduleOne.Money;
using Lithium.Modules;
using Lithium.Modules.DryingRacks;
using Lithium.Modules.PlantGrowth;
using Lithium.Modules.PropertyPrices;
using MelonLoader;


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
        ];

        public static T Get<T>() where T : ModuleBase => Modules.OfType<T>().FirstOrDefault();

        public override void OnInitializeMelon()
        {
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
                    module.Load();
                    module.Apply();
                }
                
                _isFirstStart = false;
            }
            else if (sceneName.Equals("Menu", StringComparison.OrdinalIgnoreCase) && !_isFirstStart)
            {
                _isFirstStart = true;
            }
        }
    }
}
