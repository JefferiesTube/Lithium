using Il2CppScheduleOne.ObjectScripts;
using UnityEngine;

namespace Lithium.Modules.PlantGrowth
{
    public class PotBaseValues : MonoBehaviour
    {
        public float BaseWaterDrainPerHour;

        public void Init(Pot pot)
        {
            BaseWaterDrainPerHour = pot.WaterDrainPerHour;
        }
    }
}
