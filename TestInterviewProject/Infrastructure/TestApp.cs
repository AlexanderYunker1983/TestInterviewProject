using System;
using MugenMvvmToolkit;
using TestInterviewProject.ViewModels.Main;

namespace TestInterviewProject.Infrastructure
{
    public class TestApp : MvvmApplication
    {
        public override Type GetStartViewModelType()
        {
            return typeof(MainViewModel);
        }
    }
}