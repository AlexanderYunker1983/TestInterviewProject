using System.Collections.Generic;
using OpenTK;
using TestInterviewProject.Models;

namespace TestInterviewProject.Infrastructure
{
    public interface ICoordinateHelper
    {
        List<Joint> CalculateJoints(List<Chain> chains);
        Vector2d[] GetVertexFromChains(List<Chain> chains);
    }
}