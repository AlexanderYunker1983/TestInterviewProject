using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.ViewModels;
using TestInterviewProject.Infrastructure;
using TestInterviewProject.Messages;
using TestInterviewProject.Models;

namespace TestInterviewProject.ViewModels.Report
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ICoordinateHelper coordinateHelper;

        public ReportViewModel(IEventAggregator eventAggregator, ICoordinateHelper coordinateHelper)
        {
            this.eventAggregator = eventAggregator;
            this.coordinateHelper = coordinateHelper;

            this.eventAggregator.Subscribe<ChainPositionsChanged>(OnChainPositionsChanged);
        }

        private void OnChainPositionsChanged(object sender, ChainPositionsChanged args)
        {
            if (Equals(sender, this))
            {
                return;
            }

            UpdateJoints(args.Chains);
        }

        private void UpdateJoints(IEnumerable<Chain> chains)
        {
            var chainList = chains.ToList();
            if (!chainList.Any())
            {
                return;
            }
            if (Joints.Count != chainList.Count + 1)
            {
                Joints.Clear();
                for (var index = 0; index < chainList.Count + 1; index++)
                {
                    Joints.Add(new Joint
                    {
                        X = 0.0,
                        Y = 0.0,
                        Z = 0.0,
                        Name = char.ConvertFromUtf32('A' + index)
                    });
                }
            }

            var joints = coordinateHelper.CalculateJoints(chainList);
            for (var index = 0; index < joints.Count; index++)
            {
                Joints[index].Update(joints[index]);
            }
        }

        public ObservableCollection<Joint> Joints { get; set; } = new ObservableCollection<Joint>();
    }
}