using System.Collections.Generic;
using TestInterviewProject.Models;

namespace TestInterviewProject.Infrastructure
{
    public interface IChainsBuilder
    {
        IEnumerable<Chain> GetStartChainsPosition();
    }
}