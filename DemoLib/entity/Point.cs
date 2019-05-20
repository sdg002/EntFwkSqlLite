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
        /// <summary>
        /// Helps in comparing object state before persistence and after retrieval from persistence from EF
        /// </summary>
        /// <returns></returns>
        public Point Clone()
        {
            Point pt = this.MemberwiseClone() as Point;
            return pt;
        }
        /// <summary>
        /// This helps us compare objects before persistence and after retrieval from persistence
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Point target = obj as Point;
            if (target == null) return false;
            double tolerance = 0.001;
            if (Math.Abs(target.X - this.X) > tolerance) return false;
            if (Math.Abs(target.Y - this.Y) > tolerance) return false;
            return true;
        }
    }
}
