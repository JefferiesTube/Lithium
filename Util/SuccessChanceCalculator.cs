using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using MelonLoader;
using UnityEngine;

namespace Lithium.Util
{
    public static class SuccessChanceCalculator
    {
        public static float CalculateSuccess(EDrugType drugType, EQuality quality, float qualityLevelModifier, 
            ECustomerStandard standard, string[] desires, string[] effects, CustomerAffinityData affinities,
            bool includeDrugPreference, float baseAcceptance)
        {
            float acceptance;
            
            if (desires.Length > 0)
            {
                int coveredEffects = 0;
                foreach (string desire in desires.Where(d => !string.IsNullOrEmpty(d)))
                {
                    coveredEffects += effects.Contains(desire) ? 1 : 0;
                }

                acceptance = (float)coveredEffects / desires.Length;
                Core.Logger.Msg($"Sample offering: Covered {coveredEffects} desires. Base acceptance {acceptance*100:F1}%");
            }
            else
            {
                Core.Logger.Msg($"Sample offering: No desired. Base acceptance 100%");
                acceptance = 1f;
            }

            int qualityDiff = (int)quality - (int)standard;
            Core.Logger.Msg($"Sample offering: Quality difference {qualityDiff} levels");
            acceptance += qualityLevelModifier * qualityDiff;
            Core.Logger.Msg($"Adjusted acceptance: {acceptance*100:F1}%");

            if (includeDrugPreference)
            {
                foreach (ProductTypeAffinity productAffinity in affinities.ProductAffinities)
                {
                    if (productAffinity.DrugType == drugType)
                    {
                        Core.Logger.Msg(
                            $"Sample offering: Product affinity modifier: {productAffinity.Affinity * 100:F1}%");
                        acceptance *= productAffinity.Affinity;
                        break;
                    }
                }
            }

            acceptance += baseAcceptance;
            Core.Logger.Msg($"Sample offering: Final acceptance is {Mathf.Clamp01(acceptance):F1}%");
            return Mathf.Clamp01(acceptance);
        }
    }
}
