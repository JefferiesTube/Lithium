using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Properties;

namespace Lithium.Helper
{
    public static class ProductHelper
    {
        public static bool PlayerHasSuitableProduct(this Customer customer)
        {
            List<Property> desires = customer.CustomerData.PreferredProperties.ToList();

            if (desires.Count == 0)
                return true;

            List<ProductDefinition> suitableProducts = ProductManager.DiscoveredProducts.ToList()
                .Where(p => p.Properties.ToList().Intersect(desires).Any())
                .ToList();
            
            return suitableProducts.Count != 0 || desires.Count <= 0;
        }

        public static bool DealerHasSuitableProduct(this Customer customer)
        {
            List<string> desires = customer.CustomerData.PreferredProperties
                .ToList()
                .Select(p => p.Name)
                .ToList();

            if (desires.Count == 0)
                return true;

            return customer.AssignedDealer.ItemSlots
                .ToList()
                .Any(i => i.ItemInstance is ProductItemInstance { Definition: ProductDefinition pd }
                                   && ProductMatchesDesires(pd, desires));
        }

        public static bool ProductMatchesDesires(ProductDefinition pd, List<string> desires) => pd.Properties.ToList().Select(p => p.Name).Intersect(desires).Any();

        public static string FormatDesires(CustomerData customerData) => customerData.PreferredProperties.ToList().Select(p => p.Name).SmartJoin(", ", " or ");
    }
}
