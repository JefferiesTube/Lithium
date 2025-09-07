using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.UI.Phone.Delivery;
using Lithium.Modules.Shops;

namespace Lithium.Util;

public static class DeliveryUtils
{
    public static void ApplyDeliveryOverrides()
    {
        Dictionary<string, DeliveryShop> ingameShops = UnityEngine.Object.FindObjectsOfType<DeliveryShop>(true)
            .ToList()
            .Where(s => s != null && s.MatchingShop != null)
            .ToDictionary(s => s.MatchingShop.ShopName, s => s);

        ModShopsConfiguration config = Core.Get<ModShops>().Configuration;

        foreach (KeyValuePair<string, DeliverySettings> entry in config.Deliveries)
        {
            if (ingameShops.TryGetValue(entry.Key, out DeliveryShop shop))
            {                                                               
                switch (entry.Value.Availability)
                {
                    case DeliveryAvailabilitySettings.Unchanged:
                        switch (shop.MatchingShop.ShopName)
                        {
                            case "Albert Hoover":
                                Albert albert = UnityEngine.Object.FindObjectOfType<Albert>();
                                shop.IsAvailable = albert.RelationData.RelationDelta > Supplier.DELIVERY_RELATIONSHIP_REQUIREMENT;
                                break;
                            case "Shirley Watts":
                                Shirley shirley = UnityEngine.Object.FindObjectOfType<Shirley>();
                                shop.IsAvailable = shirley.RelationData.RelationDelta > Supplier.DELIVERY_RELATIONSHIP_REQUIREMENT;
                                break;
                            case "Salvador Moreno":
                                Salvador salvador = UnityEngine.Object.FindObjectOfType<Salvador>();
                                shop.IsAvailable = salvador.RelationData.RelationDelta > Supplier.DELIVERY_RELATIONSHIP_REQUIREMENT;
                                break;
                            default:
                                shop.IsAvailable = shop.IsAvailable;
                                break;
                        }

                        continue;
                    case DeliveryAvailabilitySettings.Never:
                        shop.IsAvailable = false;
                        break;
                    case DeliveryAvailabilitySettings.Always:
                        shop.IsAvailable = true;
                        break;
                    case DeliveryAvailabilitySettings.AfterReachingXP:
                        shop.IsAvailable = LevelManager.Instance.TotalXP >= entry.Value.XPRequirement;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                shop.DeliveryFee = entry.Value.DeliveryFee;
                shop.DeliveryFeeLabel.text = $"${shop.DeliveryFee}";
                shop.gameObject.SetActive(shop.IsAvailable);
            }
        }
    }
}