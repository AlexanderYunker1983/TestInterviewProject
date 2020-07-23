using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using TestInterviewProject.Models;

namespace TestInterviewProject.Infrastructure.Impl
{
    public class CoordinateHelper : ICoordinateHelper
    {
        public List<Joint> CalculateJoints(List<Chain> chainList)
        {
            var joints = new List<Joint>();
            for (var index = 0; index < chainList.Count + 1; index++)
            {
                joints.Add(new Joint
                {
                    X = 0.0,
                    Y = 0.0,
                    Z = 0.0,
                    Name = char.ConvertFromUtf32('A' + index)
                });
            }

            joints[0].X = joints[1].X = chainList[0].Coordinate;
            joints[0].Y = 0.1;
            joints[1].Y = joints[0].Y + chainList[0].Length;

            for (var index = 1; index < chainList.Count; index++)
            {
                joints[index + 1].X = joints[index].X + chainList[index].Length * Math.Cos(chainList[index].Coordinate);
                joints[index + 1].Y = joints[index].Y + chainList[index].Length * Math.Sin(chainList[index].Coordinate);
            }

            return joints;
        }

        public Vector2d[] GetVertexFromChains(List<Chain> chains)
        {
            var joints = CalculateJoints(chains);
            joints.Reverse();

            var result = joints.Select(j => new Vector2d(j.X, j.Y)).ToArray();
            return result;
        }
    }
}