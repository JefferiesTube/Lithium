using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.UI.Phone;
using Il2CppScheduleOne.UI.Shop;
using Lithium.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lithium.Modules.Shops
{
    public class SupplierListingOverride
    {
        public Dictionary<string, float> PriceOverrides { get; set; } = [];
    }

    public class ItemListingOverride
    {
        public float Price { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ShopListing.ERestockRate RestockRate { get; set; } = ShopListing.ERestockRate.Daily;
        public int Stock { get; set; } = -1;
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

        public SupplierListingOverride Albert;
        public SupplierListingOverride Shirley;
        public SupplierListingOverride Salvador;
    }

    public class ModShops : ModuleBase<ModShopsConfiguration>
    {
        public override void Apply()
        {
            ApplyShopOverrides();

            ApplySupplierOverrides();

            Configuration.SaveConfiguration();
            //if (network && NetworkSingleton<ShopManager>.InstanceExists && this.Shop != null)
            //{
            //    NetworkSingleton<ShopManager>.Instance.SendStock(this.Shop.ShopCode, this.Item.ID, quantity);
            //}
        }

        private void ApplySupplierOverrides()
        {
            void AssertSupplierConfigEntryExists(ref SupplierListingOverride configuration, Supplier supplier)
            {
                configuration ??= new()
                {
                    PriceOverrides = supplier.OnlineShopItems.ToDictionary(
                        listing => listing.Item.ID,
                        listing => listing.Price)
                };
            }

            void ApplySupplierConfigValues(SupplierListingOverride configuration, Supplier supplier)
            {
                foreach (PhoneShopInterface.Listing listing in supplier.OnlineShopItems)
                {
                    if (configuration.PriceOverrides.TryGetValue(listing.Item.ID, out float @override))
                    {
                        listing.Item.BasePurchasePrice = @override;
                    }
                }                
            }

            Albert albert = UnityEngine.Object.FindObjectOfType<Albert>();
            AssertSupplierConfigEntryExists(ref Configuration.Albert, albert);
            ApplySupplierConfigValues(Configuration.Albert, albert);

            Shirley shirley = UnityEngine.Object.FindObjectOfType<Shirley>();
            AssertSupplierConfigEntryExists(ref Configuration.Shirley, shirley);
            ApplySupplierConfigValues(Configuration.Shirley, shirley);


            Salvador salvador = UnityEngine.Object.FindObjectOfType<Salvador>();
            AssertSupplierConfigEntryExists(ref Configuration.Salvador, salvador);
            ApplySupplierConfigValues(Configuration.Salvador, salvador);
        }

        private void ApplyShopOverrides()
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
                        Stock = listing.LimitedStock ? listing.DefaultStock : -1,
                        RestockRate = listing.RestockRate,
                    }),
            };
            shopInterface.RefreshShownItems();
            shopInterface.RefreshUnlockStatus();
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
                    continue;

                listing.LimitedStock = overrideItem.Stock >= 0;
                listing.DefaultStock = overrideItem.Stock >= 0 ? overrideItem.Stock : shopSettings.DefaultStock;
                listing.CurrentStock = listing.DefaultStock;
                listing.OverridePrice = true;
                listing.OverriddenPrice = overrideItem.Price;
                listing.RestockRate = overrideItem.RestockRate;
            }
        }
    }
}
