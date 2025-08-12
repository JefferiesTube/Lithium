using Il2CppInterop.Runtime.Injection;
using System.Linq;
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

        public List<WeightedFloat> RandomYieldsPerBudModifier = [ new(7.0f, 1.0f), new(2.0f, 2.0f), new(1.0f, 3.0f) ];
        public List<WeightedFloat> RandomYieldModifiers = [ new(7.5f, 1), new(1.0f, 0.25f), new(1.0f, 1.5f), new(0.5f, 3.0f)];
        public List<WeightedFloat> RandomQualityModifiers = [new(0, -0.5f), new(0.5f, 0), new(0.75f, 0), new(0.2f, 0.4f), new(0.01f, 0.5f)];

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
            foreach (WeightedFloat entry in Configuration.RandomYieldsPerBudModifier)
            {
                Configuration.RandomYieldPerBudPicker.Add(entry.Value, entry.Weight);
            }
            // Fallback: if user provided an empty list or omitted the field
            if (Configuration.RandomYieldsPerBudModifier.Count == 0)
            {
                Configuration.RandomYieldPerBudPicker.Add(1.0f, 7.0f);
                Configuration.RandomYieldPerBudPicker.Add(2.0f, 2.0f);
                Configuration.RandomYieldPerBudPicker.Add(3.0f, 1.0f);
            }

            Configuration.RandomYieldModifierPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomYieldModifiers)
            {
                Configuration.RandomYieldModifierPicker.Add(entry.Weight, entry.Value);
            }
            // Fallback: if empty or total weight <= 0, use default values
            if (Configuration.RandomYieldModifiers.Count == 0 ||
                Configuration.RandomYieldModifiers.Sum(e => e.Weight) <= 0f)
            {
                Configuration.RandomYieldModifierPicker.Add(7.5f, 1.0f);
                Configuration.RandomYieldModifierPicker.Add(1.0f, 0.25f);
                Configuration.RandomYieldModifierPicker.Add(1.0f, 1.5f);
                Configuration.RandomYieldModifierPicker.Add(0.5f, 3.0f);
            }

            Configuration.RandomYieldQualityPicker = new();
            foreach (WeightedFloat entry in Configuration.RandomQualityModifiers)
            {
                Configuration.RandomYieldQualityPicker.Add(entry.Weight, entry.Value);
            }
            // Fallback: if empty or total weight <= 0, use default values
            if (Configuration.RandomQualityModifiers.Count == 0 ||
                Configuration.RandomQualityModifiers.Sum(e => e.Weight) <= 0f)
            {
                Configuration.RandomYieldQualityPicker.Add(0.0f, -0.5f);
                Configuration.RandomYieldQualityPicker.Add(0.5f, 0.0f);
                Configuration.RandomYieldQualityPicker.Add(0.75f, 0.0f);
                Configuration.RandomYieldQualityPicker.Add(0.2f, 0.4f);
                Configuration.RandomYieldQualityPicker.Add(0.01f, 0.5f);
            }
        }

        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
