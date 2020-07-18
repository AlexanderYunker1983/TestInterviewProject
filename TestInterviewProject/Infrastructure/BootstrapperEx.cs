using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.WPF.Infrastructure;

namespace TestInterviewProject.Infrastructure
{
    public class BootstrapperEx : Bootstrapper<TestApp>
    {
        public BootstrapperEx(Application application,
            IIocContainer iocContainer,
            IEnumerable<Assembly> assemblies = null,
            PlatformInfo platform = null) : base(application, iocContainer, assemblies, platform)
        {
        }
    }
}