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
        private readonly ICoordinateHelper coordinateHelper;
        private Joint desiredJointPosition;

        public WorkPlaneViewModel(IChainsBuilder chainsBuilder, 
            IEventAggregator eventAggregator, 
            ICoordinateHelper coordinateHelper)
        {
            this.chainsBuilder = chainsBuilder;
            this.eventAggregator = eventAggregator;
            this.coordinateHelper = coordinateHelper;
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

        public Joint DesiredJointPosition
        {
            get => desiredJointPosition;
            set
            {
                if (!Equals(value, desiredJointPosition))
                {
                    desiredJointPosition = value;
                    OnPropertyChanged();
                    Chains = coordinateHelper.CalculateAvailableChainPositions(Chains, desiredJointPosition);
                }
            }
        }
    }
}