using MugenMvvmToolkit.ViewModels;
using TestInterviewProject.ViewModels.Report;
using TestInterviewProject.ViewModels.WorkPlane;

namespace TestInterviewProject.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
    {
        public WorkPlaneViewModel WorkPlaneViewModel { get; set; }
        public ReportViewModel ReportViewModel { get; set; }

        protected override void OnInitialized()
        {
            InitializeSubViewModels();

            base.OnInitialized();
        }

        private void InitializeSubViewModels()
        {
            WorkPlaneViewModel = GetViewModel<WorkPlaneViewModel>();
            ReportViewModel = GetViewModel<ReportViewModel>();
        }
    }
}