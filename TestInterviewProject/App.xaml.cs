using System.Windows;
using MugenMvvmToolkit;
using TestInterviewProject.Infrastructure;

namespace TestInterviewProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // ReSharper disable once AssignmentIsFullyDiscarded
            _ = new BootstrapperEx(this, new AutofacContainer());
        }
    }
}
