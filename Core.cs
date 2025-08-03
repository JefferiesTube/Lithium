using Il2CppScheduleOne.Money;
using Lithium.Modules;
using MelonLoader;


[assembly: MelonInfo(typeof(Lithium.Core), "Lithium", "1.0.0", "DerTomDer & YukiSora", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Lithium
{
    public class Core : MelonMod
    {
        private readonly List<ModuleBase> _modules = 
        [
            new ModPropertyPrices()
        ];

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Lithium initialized");
        }

        private bool _isFirstStart = true;
        
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                foreach (ModuleBase module in _modules)
                {
                    LoggerInstance.Msg($"[Lithium] Loading {module.GetType().Name}");
                    module.Load();
                    module.Apply();
                }
                
                MoneyManager.Instance.onlineBalance = 100000;
                _isFirstStart = false;
            }
            else if (sceneName.Equals("Menu", StringComparison.OrdinalIgnoreCase) && !_isFirstStart)
            {
                _isFirstStart = false;
            }
        }
    }
}