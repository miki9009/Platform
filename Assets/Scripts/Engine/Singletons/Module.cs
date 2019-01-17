
namespace Engine.Singletons
{
    public abstract class Module
    {
        public Module()
        {
            ModuleActivator.AddModule(this);
            Initialize();
        }

        public abstract void Initialize();
    }
}