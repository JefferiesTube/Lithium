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

        public static readonly List<WeightedFloat> RandomYieldsPerBudModifierDefaults = [ new(7.0f, 1.0f), new(2.0f, 2.0f), new(1.0f, 3.0f) ];
        public static readonly List<WeightedFloat> RandomYieldModifiersDefaults = [ new(7.5f, 1), new(1.0f, 0.25f), new(1.0f, 1.5f), new(0.5f, 3.0f)];
        public static readonly List<WeightedFloat> RandomQualityModifiersDefaults = [new(0, -0.5f), new(0.5f, 0), new(0.75f, 0), new(0.2f, 0.4f), new(0.01f, 0.5f)];

        public List<WeightedFloat> RandomYieldsPerBudModifier = RandomYieldsPerBudModifierDefaults;
        public List<WeightedFloat> RandomYieldModifiers = RandomYieldModifiersDefaults;
        public List<WeightedFloat> RandomQualityModifiers = RandomQualityModifiersDefaults;

        [JsonIgnore] public WeightedPicker<float> RandomYieldPerBudPicker;
        [JsonIgnore] public WeightedNormalizer RandomYieldModifierPicker;
        [JsonIgnore] public WeightedNormalizer RandomYieldQualityPicker;
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

            Configuration.RandomYieldPerBudPicker.AddRange(Configuration.RandomYieldPerBudPicker.Count == 0 
                ? ModPlantsConfiguration.RandomYieldsPerBudModifierDefaults.Select(p => new KeyValuePair<float, float>(p.Value, p.Weight))
                : Configuration.RandomYieldsPerBudModifier.Select(p => new KeyValuePair<float, float>(p.Value, p.Weight)));


            Configuration.RandomYieldModifierPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomYieldModifiers)
            {
                Configuration.RandomYieldModifierPicker.Add(entry.Weight, entry.Value);
            }
            if (Configuration.RandomYieldModifiers.Count == 0 || Configuration.RandomYieldModifiers.Sum(e => e.Weight) <= 0f)
            {
                foreach (WeightedFloat value in ModPlantsConfiguration.RandomYieldModifiersDefaults)
                {
                    Configuration.RandomYieldModifiers.Add(value);
                }
            }

            Configuration.RandomYieldQualityPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomQualityModifiers)
            {
                Configuration.RandomYieldQualityPicker.Add(entry.Weight, entry.Value);
            }
            if (Configuration.RandomQualityModifiers.Count == 0 || Configuration.RandomQualityModifiers.Sum(e => e.Weight) <= 0f)
            {
                foreach (WeightedFloat value in ModPlantsConfiguration.RandomQualityModifiersDefaults)
                {
                    Configuration.RandomYieldQualityPicker.Add(value.Weight, value.Value);
                }
            }
        }

        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
