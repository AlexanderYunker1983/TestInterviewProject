using System;
using MugenMvvmToolkit.Models;

namespace TestInterviewProject.Models
{
    public class Joint : NotifyPropertyChangedBase
    {
        private string name;
        private double x;
        private double y;
        private double z;

        public string Name
        {
            get => name;
            set
            {
                if (!string.Equals(value, name, StringComparison.Ordinal))
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public double X
        {
            get => x;
            set
            {
                if (!value.Equals(x))
                {
                    x = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Y
        {
            get => y;
            set
            {
                if (!value.Equals(y))
                {
                    y = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Z
        {
            get => z;
            set
            {
                if (!value.Equals(z))
                {
                    z = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Update(Joint joint)
        {
            X = joint.X;
            Y = joint.Y;
            Z = joint.Z;
        }
    }
}