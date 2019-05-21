using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestProject1
{
    [TestClass]
    public class PerformanceTests
    {
        public PerformanceTests()
        {
            CountOfPoints = 100;
        }
        public int CountOfPoints { get; set; }
        [TestMethod]
        public void Query_For_TriangleGiven_Point()
        {
            var pts=utils.Util.CreateRandomPoints(0, 100, CountOfPoints);
            var triangles=utils.Util.FindAllTriangles(pts);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.SaveChanges();

        }
    }
}
