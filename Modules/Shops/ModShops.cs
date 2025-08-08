using Il2CppScheduleOne.UI.Shop;
using Lithium.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lithium.Modules.Shops
{
    public class ItemListingOverride
    {
        public float Price { get; set; }
        public ShopListing.ERestockRate RestockRate { get; set; } = ShopListing.ERestockRate.Daily;
        public int Stock { get; set; } = 0;
        public bool LimitedStock { get; set; } = false;
    }

    public class ShopListingSettings
    {
        [JsonProperty(Order = 1)]
        public bool Override { get; set; } = false;
        [JsonProperty(Order = 2)]
        public int DefaultStock { get; set; } = -1;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Order = 3)]
        public ShopInterface.EPaymentType PaymentType { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Order = 4)]
        public ShopListing.ERestockRate RestockRate { get; set; } = ShopListing.ERestockRate.Daily;

        [JsonProperty(Order = 5)]
        public List<string> RemovedItems = [];

        [JsonProperty(Order = 6)]
        public Dictionary<string, ItemListingOverride> ItemOverrides = [];
    }

    public class ModShopsConfiguration : ModuleConfiguration
    {
        public override string Name => "Shops";

        public ShopListingSettings ThriftyThreads;
        public ShopListingSettings CokeSupplier;
        public ShopListingSettings MethSupplier;
        public ShopListingSettings WeedSupplier;
        public ShopListingSettings Boutique;
        public ShopListingSettings DarkMarket;
        public ShopListingSettings GasStation;
        public ShopListingSettings CentralGasStation;
        public ShopListingSettings DansHardware;
        public ShopListingSettings HandyHanks;
    }

    public class ModShops : ModuleBase<ModShopsConfiguration>
    {
        public override void Apply()
        {
            List<ShopInterface> shop = UnityEngine.Object.FindObjectsOfType<ShopInterface>().ToList();
            foreach (ShopInterface shopInterface in shop)
            {
                if (shopInterface == null)
                    continue;
                List<ShopListing> listings = shopInterface.Listings.ToList();
                if (listings == null)
                    continue;
                switch (shopInterface.ShopCode)
                {
                    case "thrifty_threads":
                        AssertConfigurationEntries(ref Configuration.ThriftyThreads, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.ThriftyThreads);
                        break;
                    case "coke_shop":
                        AssertConfigurationEntries(ref Configuration.CokeSupplier, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.CokeSupplier);
                        break;
                    case "meth_shop":
                        AssertConfigurationEntries(ref Configuration.MethSupplier, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.MethSupplier);
                        break;
                    case "weed_shop":
                        AssertConfigurationEntries(ref Configuration.WeedSupplier, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.WeedSupplier);
                        break;
                    case "boutique":
                        AssertConfigurationEntries(ref Configuration.Boutique, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.Boutique);
                        break;
                    case "dark_market_shop":
                        AssertConfigurationEntries(ref Configuration.DarkMarket, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.DarkMarket);
                        break;
                    case "gas_mart_west":
                        AssertConfigurationEntries(ref Configuration.GasStation, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.GasStation);
                        break;
                    case "gas_mart_central":
                        AssertConfigurationEntries(ref Configuration.CentralGasStation, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.CentralGasStation);
                        break;
                    case "dans_hardware":
                        AssertConfigurationEntries(ref Configuration.DansHardware, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.DansHardware);
                        break;
                    case "handy_hanks":
                        AssertConfigurationEntries(ref Configuration.HandyHanks, shopInterface, listings);
                        if (Configuration.Enabled)
                            ApplyShopSettings(listings, shopInterface, Configuration.HandyHanks);
                        break;
                }
            }

            Configuration.SaveConfiguration();
            //if (network && NetworkSingleton<ShopManager>.InstanceExists && this.Shop != null)
            //{
            //    NetworkSingleton<ShopManager>.Instance.SendStock(this.Shop.ShopCode, this.Item.ID, quantity);
            //}
        }

        private void AssertConfigurationEntries(ref ShopListingSettings configSetting,
            ShopInterface shopInterface, List<ShopListing> listings)
        {
            configSetting ??= new()
            {
                Override = false,
                DefaultStock = -1,
                PaymentType = shopInterface.PaymentType,
                ItemOverrides = listings.ToDictionary(
                    listing => listing.Item.ID,
                    listing => new ItemListingOverride
                    {
                        Price = listing.Price,
                        Stock = listing.DefaultStock,
                        RestockRate = listing.RestockRate,
                        LimitedStock = listing.LimitedStock
                    }),
            };
        }

        private void ApplyShopSettings(List<ShopListing> listings, ShopInterface shopInterface,
            ShopListingSettings shopSettings)
        {
            if (!shopSettings.Override)
                return;
            shopInterface.PaymentType = shopSettings.PaymentType;
            foreach (ShopListing listing in listings)
            {
                if (!shopSettings.ItemOverrides.TryGetValue(listing.Item.ID, out ItemListingOverride overrideItem))
                {
                    
                }
                //listing.LimitedStock = && overrideItem.LimitedStock;
            }

            listings[0].RestockRate = shopSettings.RestockRate;

        }
    }
}
