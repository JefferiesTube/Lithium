namespace Lithium.Modules
{
    public abstract class ModuleBase
    {
        public abstract void Load();
        public abstract void Apply();
    }

    public abstract class ModuleBase<TConfiguration> : ModuleBase
        where TConfiguration : ModuleConfiguration
    {
        protected TConfiguration Configuration { get; private set; }
        protected ModuleBase()
        {

        }

        public override void Load()
        {
            Configuration = Activator.CreateInstance<TConfiguration>();
            Configuration.LoadConfiguration();
        }
    }
}
