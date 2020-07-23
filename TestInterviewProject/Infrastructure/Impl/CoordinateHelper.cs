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

        public IEnumerable<Chain> CalculateAvailableChainPositions(IEnumerable<Chain> chains, Joint desiredJointPosition)
        {
            var chainList = chains.ToList();
            var oldChains = chainList.ToList();
            var jointIndex = chainList.Count -  int.Parse(desiredJointPosition.Name);
            if (jointIndex < 1)
            {
                jointIndex = 1;
            }

            var lengthMax = 0.0;
            if (jointIndex > 1)
            {
                for (int index = 1; index < jointIndex; index++)
                {
                    lengthMax += oldChains[index].Length;
                }
            }

            if (desiredJointPosition.Y > 0.15 + lengthMax)
            {
                desiredJointPosition.Y = 0.15 + lengthMax;
            }
            if (desiredJointPosition.Y < 0.15 - lengthMax)
            {
                desiredJointPosition.Y = 0.15 - lengthMax;
            }

            var oldDelta = 0.0;
            var delta = 1.0;
            while (Math.Abs(oldDelta - delta) > 1e-6)
            {
                oldDelta = delta;
                chainList = CalculateByIteration(oldChains, desiredJointPosition, jointIndex);
                delta = CalculateDelta(oldChains, jointIndex);
            }

            return chainList;
        }

        private List<Chain> CalculateByIteration(List<Chain> oldChains, Joint desiredJointPosition, int jointIndex)
        {
            var m1Koef = 0.5;
            var m2Koef = 0.5;

            var coordsDelta = new double[oldChains.Count];
            coordsDelta[0] = 1 * (jointIndex < 2 ? 1 : 1e-2) * (m1Koef * (desiredJointPosition.X - CalcLengthX(oldChains, jointIndex)) +
                                                 m2Koef * (desiredJointPosition.Y - CalcLengthY(oldChains, jointIndex)));

            for (var index = 1; index < oldChains.Count; index++)
            {
                coordsDelta[index] =
                    -1 * (m1Koef * (desiredJointPosition.X - CalcLengthX(oldChains, jointIndex)) * oldChains[index].Length *
                        Math.Sin(oldChains[index].Coordinate) - m2Koef *
                        (desiredJointPosition.Y - CalcLengthY(oldChains, jointIndex)) * oldChains[index].Length *
                        Math.Cos(oldChains[index].Coordinate));
            }
            
            for (var index = 0; index < jointIndex; index++)
            {
                var oldValue = oldChains[index];
                oldChains[index] = new Chain
                {
                    Coordinate = oldValue.Coordinate + coordsDelta[index],
                    Length = oldValue.Length
                };
            }

            return oldChains;
        }

        private double CalculateDelta(List<Chain> oldChains, int jointIndex)
        {
            var result = 0.0;
            for (var index = 0; index < jointIndex; index++)
            {
                result += Math.Pow(oldChains[index].Coordinate, 2);
            }

            result *= 0.5;
            return result;
        }

        private double CalcLengthX(List<Chain> oldChains, int jointIndex)
        {
            var result = 0.0;
            for (var index = 0; index < jointIndex; index++)
            {
                result += index == 0 ? oldChains[index].Coordinate : oldChains[index].Length * Math.Cos(oldChains[index].Coordinate);
            }
            return result;
        }

        private double CalcLengthY(List<Chain> oldChains, int jointIndex)
        {
            var result = 0.0;
            for (var index = 0; index < jointIndex; index++)
            {
                result += index == 0 ? 0.15 : oldChains[index].Length * Math.Sin(oldChains[index].Coordinate);
            }
            return result;
        }
    }
}