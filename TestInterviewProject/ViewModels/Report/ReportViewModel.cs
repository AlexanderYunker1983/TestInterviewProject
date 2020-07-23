using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.ViewModels;
using TestInterviewProject.Messages;
using TestInterviewProject.Models;

namespace TestInterviewProject.ViewModels.Report
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;

        public ReportViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

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

            Joints[0].X = Joints[1].X = chainList[0].Coordinate;
            Joints[0].Y = 0.1;
            Joints[1].Y = Joints[0].Y + chainList[0].Length;

            for (var index = 1; index < chainList.Count; index++)
            {
                Joints[index + 1].X = Joints[index].X + chainList[index].Length * Math.Cos(chainList[index].Coordinate);
                Joints[index + 1].Y = Joints[index].Y + chainList[index].Length * Math.Sin(chainList[index].Coordinate);
            }
        }

        public ObservableCollection<Joint> Joints { get; set; } = new ObservableCollection<Joint>();
    }
}