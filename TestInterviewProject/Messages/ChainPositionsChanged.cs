using System.Collections.Generic;
using TestInterviewProject.Models;

namespace TestInterviewProject.Messages
{
    public class ChainPositionsChanged
    {
        public ChainPositionsChanged(IEnumerable<Chain> chains)
        {
            Chains = chains;
        }

        public IEnumerable<Chain> Chains { get; set; }
    }
}