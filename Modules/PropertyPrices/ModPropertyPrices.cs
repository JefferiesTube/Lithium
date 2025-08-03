using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.Money;
using Il2CppTMPro;
using Property = Il2CppScheduleOne.Property.Property;

namespace Lithium.Modules.PropertyPrices
{
    public class ModPropertyPricesConfiguration : ModuleConfiguration
    {
        public override string Name => "Property Prices";
        public Dictionary<string, int> PropertyPrices { get; set; } = new Dictionary<string, int>
        {
            { "RV", 0 },
            { "Motel Room", 75 },
            { "Sweatshop", 800 },
            { "Storage Unit", 5000 },
            { "Bungalow", 6000 },
            { "Barn", 25000 },
            { "Docks Warehouse", 50000 },

            { "Laundromat", 4000 },
            { "Post Office", 10000 },
            { "Car Wash", 20000 },
            { "Taco Ticklers", 50000 },
            { "Manor", 100000 },
            };
    }

    public class ModPropertyPrices : ModuleBase<ModPropertyPricesConfiguration>
    {
        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;

            ChangePropertyPrices();
            UpdateMissMingDialog();
        }

        private void ChangePropertyPrices()
        {
            Property[] props = UnityEngine.Object.FindObjectsOfType<Property>();
            foreach (Property prop in props)
            {
                if (!Configuration.PropertyPrices.TryGetValue(prop.PropertyName, out int price))
                {
                    MelonLoader.MelonLogger.Warning($"Property {prop.PropertyName} not found in configuration. Skipping.");
                    continue;
                }

                prop.Price = price;
               
                if (prop.ForSaleSign != null)
                    prop.ForSaleSign.transform.Find("Price").GetComponent<TextMeshPro>().text = MoneyManager.FormatAmount(price);
                if (prop.ListingPoster != null)
                    prop.ListingPoster.Find("Price").GetComponent<TextMeshPro>().text = MoneyManager.FormatAmount(price);
            }
        }

        private void UpdateMissMingDialog()
        {
            DialogueController_Ming[] dialogueControllers = UnityEngine.Object.FindObjectsOfType<DialogueController_Ming>();
            foreach (DialogueController_Ming item in dialogueControllers)
            {
                item.Price = item.gameObject.transform.parent?.name switch
                {
                    "Ming" => Configuration.PropertyPrices["Sweatshop"],
                    "Donna" => Configuration.PropertyPrices["Motel Room"],
                    _ => item.Price
                };
            }
        }
    }
}
