﻿using Scumle.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Scumle.Model
{
    public class ConnectionPoint : ModelBase, IPoint
    {

        public ConnectionPoint(Shape _shape, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            Shape = _shape;
            Horizontal = horizontal;
            Vertical = vertical;
            this.ShapeColor = new SolidColorBrush(Color.FromRgb(47, 79, 79));
            storeShape = _shape;
            
        }
        private Brush _shapeColor;

        // For XML Serialization
        public ConnectionPoint() { }

        public HorizontalAlignment Horizontal { get; set; }
        public VerticalAlignment Vertical { get; set; }

        [XmlIgnore]
        public IShape Shape { get; private set; }

        [XmlIgnore]
        public Brush ShapeColor
        {
            get { return _shapeColor; }
            set { _shapeColor = value; }
        }

        // For Serialization
        public Shape storeShape { set; get; }

        public double CenterX
        {
            get { return getCenterX(); }
        }

        public double CenterY
        {
            get { return getCenterY(); }
        }


        private double getCenterX()
        {
            if (Horizontal == HorizontalAlignment.Right)
            {
                return Shape.X + Shape.Width;
            }
            else if (Horizontal == HorizontalAlignment.Center)
            {
                return Shape.X + Shape.Width / 2;
            }
            else
            {
                return Shape.X;
            }
        }

        private double getCenterY()
        {
            if (Vertical == VerticalAlignment.Bottom)
            {
                return Shape.Y + Shape.Height;
            }
            else if (Vertical == VerticalAlignment.Center)
            {
                return Shape.Y + Shape.Height / 2;
            }
            else
            {
                return Shape.Y;
            }
        }

    }
}
