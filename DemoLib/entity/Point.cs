using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.entity
{
    /// <summary>
    /// Represents a 2 dimensional point
    /// </summary>
    public class Point : ElementBase
    {
        double _length=-1;
        public Point()
        {

        }
        public Point(double x,double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
        /// <summary>
        /// If the Point were treated as a Vector which originated from 0,0 then the Length returns the 
        /// Euclidean distance from the origin
        /// </summary>
        public double Length
        {
            get
            {
                if (_length == -1)
                {
                    _length = Math.Sqrt(X * X + Y * Y);
                }
                return _length;
            }
        }

        public override string ToString()
        {
            return $"ID={ID}  X,Y={X:.00},{Y:.00}";
        }
    }
}
