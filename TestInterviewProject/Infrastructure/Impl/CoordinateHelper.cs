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
            
            if (jointIndex == 0 || jointIndex == 1)
            {
                var oldValue = chainList[0];
                chainList[0] = new Chain
                {
                    Length = oldValue.Length,
                    Index = oldValue.Index,
                    Coordinate = desiredJointPosition.X
                };
            }

            if (jointIndex == 2)
            {
                if (desiredJointPosition.Y > 0.15 + oldChains[1].Length)
                {
                    desiredJointPosition.Y = 0.15 + oldChains[1].Length;
                }
                if (desiredJointPosition.Y < 0.15 - oldChains[1].Length)
                {
                    desiredJointPosition.Y = 0.15 - oldChains[1].Length;
                }

                var angle = Math.Asin((desiredJointPosition.Y - 0.15) / oldChains[1].Length);
                var x = desiredJointPosition.X - oldChains[1].Length * Math.Cos(angle);

                chainList[0] = new Chain
                {
                    Length = oldChains[0].Length,
                    Index = oldChains[0].Index,
                    Coordinate = x
                };
                chainList[1] = new Chain
                {
                    Length = oldChains[1].Length,
                    Index = oldChains[1].Index,
                    Coordinate = angle
                };
            }

            if (jointIndex == 3)
            {
                if (desiredJointPosition.Y > 0.15 + oldChains[1].Length + oldChains[2].Length)
                {
                    desiredJointPosition.Y = 0.15 + oldChains[1].Length + oldChains[2].Length;
                }
                if (desiredJointPosition.Y < 0.15 - oldChains[1].Length - oldChains[2].Length)
                {
                    desiredJointPosition.Y = 0.15 - oldChains[1].Length - oldChains[2].Length;
                }

                for (int index = 0; index < 100; index++)
                {
                    chainList = CalculateByIteration(oldChains, desiredJointPosition);
                }
            }

            return chainList;
        }

        private List<Chain> CalculateByIteration(List<Chain> oldChains, Joint desiredJointPosition)
        {
            var m1Koef = 0.1;
            var m2Koef = 0.1;

            var x0 = 1 * (m1Koef *(desiredJointPosition.X - CalcLengthX(oldChains)) + m2Koef * (desiredJointPosition.Y - CalcLengthY(oldChains)));
            var phi1 = -1 * (m1Koef * (desiredJointPosition.X - CalcLengthX(oldChains)) * oldChains[1].Length * Math.Sin(oldChains[1].Coordinate) - m2Koef * (desiredJointPosition.Y - CalcLengthY(oldChains)) * oldChains[1].Length * Math.Cos(oldChains[1].Coordinate));
            var phi2 = -1 * (m1Koef * (desiredJointPosition.X - CalcLengthX(oldChains)) * oldChains[2].Length * Math.Sin(oldChains[2].Coordinate) - m2Koef * (desiredJointPosition.Y - CalcLengthY(oldChains)) * oldChains[2].Length * Math.Cos(oldChains[2].Coordinate));

            for (var index = 0; index < oldChains.Count; index++)
            {
                var oldValue = oldChains[index];
                oldChains[index] = new Chain
                {
                    Coordinate = oldValue.Coordinate + (index == 0 ? x0 : index == 1 ? phi1 : phi2),
                    Length = oldValue.Length,
                    Index = oldValue.Index
                };
            }

            return oldChains;
        }

        private double CalcLengthX(List<Chain> oldChains)
        {
            var result = oldChains[0].Coordinate + oldChains[1].Length * Math.Cos(oldChains[1].Coordinate) + oldChains[2].Length * Math.Cos(oldChains[2].Coordinate);
            return result;
        }

        private double CalcLengthY(List<Chain> oldChains)
        {
            var result = 0.15 + oldChains[1].Length * Math.Sin(oldChains[1].Coordinate) + oldChains[2].Length * Math.Sin(oldChains[2].Coordinate);
            return result;
        }
    }
}