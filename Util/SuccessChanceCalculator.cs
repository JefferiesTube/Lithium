using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using UnityEngine;

namespace Lithium.Util
{
    public static class SuccessChanceCalculator
    {
        public static float CalculateSuccess(EDrugType drugType, EQuality quality, ECustomerStandard standard, string[] desires, string[] effects, CustomerAffinityData affinities)
        {
            float acceptance;
            
            if (desires.Length > 0)
            {
                int coveredEffects = 0;
                foreach (string desire in desires)
                {
                    coveredEffects += effects.Contains(desire) ? 1 : 0;
                }

                acceptance = (float)coveredEffects / desires.Length;
            }
            else
            {
                acceptance = 1f;
            }

            int qualityDiff = (int)quality - (int)standard;
            acceptance += 0.2f * qualityDiff;

            foreach (ProductTypeAffinity productAffinity in affinities.ProductAffinities)
            {
                if (productAffinity.DrugType == drugType)
                {
                    acceptance *= productAffinity.Affinity;
                    break;
                }
            }

            return Mathf.Clamp01(acceptance);
        }
    }
}
