using System;
using System.Collections.Generic;
using System.Text;

namespace DemoLib.entity
{
    public class Triangle : ElementBase
    {
        public double[] Angles { get;  }
        public double[] Sides { get; set; }
        public ICollection<TriangleVertex> Vertices { get; set; }
    }
}
