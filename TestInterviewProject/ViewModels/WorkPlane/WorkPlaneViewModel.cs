using System.Collections.Generic;
using MugenMvvmToolkit.ViewModels;
using TestInterviewProject.Infrastructure;
using TestInterviewProject.Models;

namespace TestInterviewProject.ViewModels.WorkPlane
{
    public class WorkPlaneViewModel : ViewModelBase
    {
        private IEnumerable<Chain> chains;

        private readonly IChainsBuilder chainsBuilder;

        public WorkPlaneViewModel(IChainsBuilder chainsBuilder)
        {
            this.chainsBuilder = chainsBuilder;
        }

        protected override void OnInitialized()
        {
            Chains = chainsBuilder.GetStartChainsPosition();

            base.OnInitialized();
        }

        public IEnumerable<Chain> Chains
        {
            get => chains;
            set
            {
                if (!Equals(value, chains))
                {
                    chains = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}