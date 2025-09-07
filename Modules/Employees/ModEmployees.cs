using Il2CppInterop.Runtime.Injection;
using Lithium.Modules.Employees.Patches;
using Lithium.Modules.PlantGrowth.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppScheduleOne.Employees;

namespace Lithium.Modules.Employees
{
    public class BotanistsConfiguration
    {
        public int MaxAssignedPots = 8;
        public float WalkSpeed = 1.2f;
        public int DailyWage = 200;
        public float SoilPourTime = 10f;
        public float WaterPourTime = 10f;
        public float AdditivePourTime = 10f;
        public float SeedSowTime = 15f;
        public float HarvestTime = 15f;
    }

    public class ChemistConfiguration
    {
        public int MaxStations = 6;
        public int DailyWage = 300;
        public float WalkSpeed = 1.2f;
    }

    public class PackagerConfiguration
    {
        public int MaxStations = 3;
        public int MaxRoutes = 10;
        public float PackagingSpeedMultiplier = 2f;
        public int DailyWage = 200;
        public float WalkSpeed = 1.2f;
    }

    public class CleanerConfiguration
    {
        public int MaxBins = 3;
        public int DailyWage = 100;
        public float WalkSpeed = 1.2f;
    }

    public class ModEmployeesConfiguration : ModuleConfiguration
    {
        public override string Name => "Employees";
        public BotanistsConfiguration Botanists = new BotanistsConfiguration();
        public ChemistConfiguration Chemists = new ChemistConfiguration();
        public PackagerConfiguration Packagers = new PackagerConfiguration();
        public CleanerConfiguration Cleaners = new CleanerConfiguration();
    }
    
    public class ModEmployees : ModuleBase<ModEmployeesConfiguration>
    {
        public static readonly HashSet<Employee> ConfiguredEmployees = [];

        public override void Apply()
        {
            if (!Configuration.Enabled)
                return;
        }
    }
}
