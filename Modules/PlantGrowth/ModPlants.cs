using Il2CppInterop.Runtime.Injection;
using Lithium.Modules.PlantGrowth.Behaviours;
using Lithium.Util;
using Newtonsoft.Json;

namespace Lithium.Modules.PlantGrowth
{
    public class WeightedFloat
    {
        public float Weight;
        public float Value;

        [JsonConstructor]
        public WeightedFloat(float weight, float value)
        {
            Weight = weight;
            Value = value;
        }
    }

    public class ModPlantsConfiguration : ModuleConfiguration
    {
        public override string Name => "Plants";

        public float GrowthModifier = 1f;
        public float WaterDrainModifier = 1f;

        public List<WeightedFloat> RandomYieldsPerBudModifier = [ new(1, 1) ];
        public List<WeightedFloat> RandomYieldModifiers = [ new(1, 1)];
        public List<WeightedFloat> RandomQualityModifiers = [new(1, 0)];

        [JsonIgnore] public WeightedPicker<float> RandomYieldPerBudPicker;
        [JsonIgnore] public WeightedPicker<float> RandomYieldModifierPicker;
        [JsonIgnore] public WeightedPicker<float> RandomYieldQualityPicker;
    }

    public class ModPlants : ModuleBase<ModPlantsConfiguration>
    {
        public ModPlants()
        {
            ClassInjector.RegisterTypeInIl2Cpp<PlantModified>();
            ClassInjector.RegisterTypeInIl2Cpp<PlantBaseQuality>();
            ClassInjector.RegisterTypeInIl2Cpp<PotBaseValues>();
        }

        protected override void OnBeforeConfigurationLoaded()
        {
            base.OnBeforeConfigurationLoaded();
            Configuration.RandomYieldsPerBudModifier.Clear();
            Configuration.RandomYieldModifiers.Clear();
            Configuration.RandomQualityModifiers.Clear();
        }

        public override void Load()
        {
            base.Load();
            Configuration.RandomYieldPerBudPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomYieldsPerBudModifier)
            {
                Configuration.RandomYieldPerBudPicker.Add(entry.Value, entry.Weight);
            }

            Configuration.RandomYieldModifierPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomYieldModifiers)
            {
                Configuration.RandomYieldModifierPicker.Add(entry.Value, entry.Weight);
            }

            Configuration.RandomYieldQualityPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomQualityModifiers)
            {
                Configuration.RandomYieldQualityPicker.Add(entry.Value, entry.Weight);
            }
        }

        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
