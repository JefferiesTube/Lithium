using Il2CppScheduleOne.ItemFramework;

namespace Lithium.Modules.StackSizes
{
    public class ModStackSizesConfiguration : ModuleConfiguration
    {
        public override string Name => "StackSizes";

        public Dictionary<EItemCategory, int> CategorySizes { get; set; } = new Dictionary<EItemCategory, int>
        {
            { EItemCategory.Product, 20 },
            { EItemCategory.Packaging, 20 },
            { EItemCategory.Agriculture, 20 },
            { EItemCategory.Tools, 10 },
            { EItemCategory.Furniture, 10 },
            { EItemCategory.Lighting, 10 },
            { EItemCategory.Cash, 1000 },
            { EItemCategory.Consumable, 20 },
            { EItemCategory.Equipment, 20 },
            { EItemCategory.Ingredient, 20 },
            { EItemCategory.Decoration, 10 },
            { EItemCategory.Clothing, 10 },
            { EItemCategory.Storage, 10 },
        };

        public Dictionary<string, int> ItemOverrides { get; set; } = [];
        public List<string> IgnoredItems { get; set; } = [];

        public int MixingStation { get; set; } = 20;
        public int DryingRack { get; set; } = 20;
    }

    public class ModStackSizes : ModuleBase<ModStackSizesConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
      
    }
}
