using MugenMvvmToolkit;
using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models.IoC;
using TestInterviewProject.Infrastructure.Impl;
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

            iocContainer.Bind<IChainsBuilder, DefaultChainBuilder>(DependencyLifecycle.SingleInstance);
            iocContainer.Bind<ICoordinateHelper, CoordinateHelper>(DependencyLifecycle.SingleInstance);

            return true;
        }

        public void Unload(IModuleContext context)
        {

        }
    }
}