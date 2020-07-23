using System.Collections.Generic;
using MugenMvvmToolkit.ViewModels;
using TestInterviewProject.Models;

namespace TestInterviewProject.ViewModels.WorkPlane
{
    public class WorkPlaneViewModel : ViewModelBase
    {
        public List<Chain> Chains { get; set; } = new List<Chain>
        {
            new Chain
            {
                Coordinate = 0.1,
                Index = 0,
                Length = 0.05
            },
            new Chain
            {
                Coordinate = 0.5,
                Index = 1,
                Length = 0.3
            },
            new Chain
            {
                Coordinate = 0,
                Index = 1,
                Length = 0.3
            },
        };
    }
}