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
            if (!config.Enabled || !config.SampleOffering.Enabled)
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
                    config.SampleOffering.QualityLevelModifier,
                    __instance.CustomerData.Standards,
                    desires, 
                    productEffects,
                    __instance.CustomerData.DefaultAffinityData,
                    config.SampleOffering.IncludeDrugPreference,
                    config.SampleOffering.BaseAcceptance);
            }

            __result = sum / items.Count;
            return false;
        }
    }
}