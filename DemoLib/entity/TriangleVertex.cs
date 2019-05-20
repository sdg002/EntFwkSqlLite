using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("PointID")]
        public int ParentID { get; set; }
        public Triangle Parent { get; set; }
        /// <summary>
        /// A vertex of the triangle
        /// </summary>
        public Point Vertex { get; set; }
        /// <summary>
        /// ID of the point it points to
        /// </summary>
        [ForeignKey("PointID")]
        public int PointID { get; set; }
    }
}
