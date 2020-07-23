using System.Collections.Generic;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.ViewModels;
using TestInterviewProject.Infrastructure;
using TestInterviewProject.Messages;
using TestInterviewProject.Models;

namespace TestInterviewProject.ViewModels.WorkPlane
{
    public class WorkPlaneViewModel : ViewModelBase
    {
        private IEnumerable<Chain> chains;

        private readonly IChainsBuilder chainsBuilder;
        private readonly IEventAggregator eventAggregator;

        public WorkPlaneViewModel(IChainsBuilder chainsBuilder, IEventAggregator eventAggregator)
        {
            this.chainsBuilder = chainsBuilder;
            this.eventAggregator = eventAggregator;
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

                    eventAggregator.Publish(this, new ChainPositionsChanged(value));
                }
            }
        }
    }
}