using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Properties;
using Lithium.Util;
using MelonLoader;
using UnityEngine;

namespace Lithium.Modules.Customers.Patches
{
    [HarmonyPatch(typeof(Customer), nameof(Customer.GetSampleSuccess))]

    public class CustomerSampleSuccessPatch
    {
        [HarmonyPrefix]
        static bool Prefix(Customer __instance, Il2CppSystem.Collections.Generic.List<ItemInstance> items,
            float price, ref float __result)
        {
            ModCustomersConfiguration config = Core.Get<ModCustomers>().Configuration;
            if (!config.Enabled)
                return true;

            float sum = 0;
            
            foreach (ItemInstance item in items)
            {
                ProductDefinition productDefinition = item.definition.TryCast<ProductDefinition>();
                ProductItemInstance productItemInstance = item.TryCast<ProductItemInstance>();


                string[] desires = new string[__instance.CustomerData.PreferredProperties.Count];
                int c = 0;
                foreach (Property property in __instance.CustomerData.PreferredProperties)
                {
                    desires[0] = property.Name;
                    c++;
                }

                string[] productEffects = new string[productDefinition.Properties.Count];
                c = 0;
                foreach (Property property in productDefinition.Properties)
                {
                    productEffects[0] = property.Name;
                    c++;
                }

                sum += SuccessChanceCalculator.CalculateSuccess(
                    productDefinition.DrugType,  
                    productItemInstance.Quality,
                    __instance.CustomerData.Standards,
                    desires, 
                    productEffects,
                    __instance.CustomerData.DefaultAffinityData);
            }

            __result = sum / items.Count;
            return false;
        }

        private static float CalculateSuccess(Customer customer, ProductDefinition product,
            ProductItemInstance consumedSample)
        {
            float acceptance;
            Il2CppSystem.Collections.Generic.List<string> desires = new();
            foreach (Property property in customer.CustomerData.PreferredProperties)
            {
                desires.Add(property.Name);
            }

            Il2CppSystem.Collections.Generic.List<string> productEffects = new();
            foreach (Property property in product.Properties)
            {
                productEffects.Add(property.Name);
            }

            if (desires.Count > 0)
            {
                int coveredEffects = 0;
                foreach (string desire in desires)
                {
                    coveredEffects += productEffects.Contains(desire) ? 1 : 0;
                }

                acceptance = (float)coveredEffects / desires.Count;
            }
            else
            {
                acceptance = 1f;
            }

            int qualityDiff = (int)consumedSample.Quality - (int)customer.CustomerData.Standards;
            acceptance += 0.2f * qualityDiff;

            foreach (ProductTypeAffinity productAffinity in customer.currentAffinityData.ProductAffinities)
            {
                if (productAffinity.DrugType == product.DrugType)
                    acceptance *= productAffinity.Affinity;
            }

            MelonLogger.Warning($"\x1B[38;5;226m Sample offered. Acceptance: {acceptance}");
            return Mathf.Clamp01(acceptance);
        }
    }
}