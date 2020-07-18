using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models.IoC;
using YLocalization;
using YMugenExtensions;

namespace TestInterviewProject.Infrastructure
{
    public class TestAppModule : IModule
    {
        public int Priority => ApplicationSettings.ModulePriorityDefault + 1;

        public bool Load(IModuleContext context)
        {
            var iocContainer = context.IocContainer;
            var locManager = new MugenLocalizationManager();
            iocContainer.BindToConstant(typeof(ILocalizationManager), locManager);
            locManager.AddAssembly("TestInterviewProject", "Properties.Resources");

            return true;
        }

        public void Unload(IModuleContext context)
        {

        }
    }
}