﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Scumle.Model
{
    class Shape
    {   

        public Shape(double X, double Y, String Name)
        {
            this.X = X;
            this.Y = Y;
            this.Width = 25;
            this.Height = 25;
            this.Name = Name;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public String Name { get; set; }

        public void MoveDelta(double X, double Y)
        {
            this.X += X;
            this.Y += Y;
        }

    }
}
