using System;
using System.Collections.Generic;
using System.Text;

namespace DemoLib.entity
{
    /// <summary>
    /// Represents a single vertex of a Triangle
    /// </summary>
    public class TriangleVertex : ElementBase
    {
        /// <summary>
        /// ID of parent triangle
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// A vertex of the triangle
        /// </summary>
        public Point Vertex { get; set; }
    }
}
