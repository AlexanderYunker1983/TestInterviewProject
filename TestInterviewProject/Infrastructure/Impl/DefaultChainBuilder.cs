using System.Collections.Generic;
using TestInterviewProject.Models;

namespace TestInterviewProject.Infrastructure.Impl
{
    public class DefaultChainBuilder : IChainsBuilder
    {
        public IEnumerable<Chain> GetStartChainsPosition()
        {
            var result = new List<Chain>
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

            return result;
        }
    }
}