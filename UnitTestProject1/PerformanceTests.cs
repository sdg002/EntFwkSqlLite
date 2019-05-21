using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        /// <summary>
        /// In this test we are picking a Point at random and finding all Triangles which contain that Point
        /// In this test we are verifying that when querying for a Triangle with a given point, 
        /// Sqllite foreign keys are being used to speed up querying
        /// </summary>
        [TestMethod]
        public void Query_For_TriangleGiven_Point_Using_SqlLite_WORK_IN_PROGRESS()
        {
            Random rnd = new Random(DateTime.Now.Second);
            var pts =utils.Util.CreateRandomPoints(0, 100, CountOfPoints);
            var triangles=utils.Util.FindAllTriangles(pts);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.Triangles.AddRange(triangles);
            ctx.SaveChanges();
            int[] idsOfPointsRandom = pts.Select(p => p.ID).OrderBy(id => rnd.Next()).ToArray();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var q = ctx.Triangles.AsQueryable();
            foreach (int idOfPoint in idsOfPointsRandom)
            {
                var tri = from t in ctx.Triangles join tv in ctx.TriangleVertices on t.ID equals tv.ParentID where tv.PointID == idOfPoint select t;
                    //ctx.Triangles.Single(t=>t.ve)
                    //AsQueryable().in.Single(t => t.Vertices.Any(tv => tv.Vertex.ID == idOfPoint)).ToArray();
                Assert.IsTrue(tri.Count() > 0);
                //Assert.IsTrue(tri.Vertices.Any(tv => tv.PointID == idOfPoint));
            }
            sw.Stop();
            Trace.WriteLine($"Time taken to query for '{idsOfPointsRandom.Length}' points and '{triangles.Length}' triangles was {sw.ElapsedMilliseconds} ms ");
        }
        /// <summary>
        /// In this test we are picking a Point at random and finding all Triangles which contain that Point
        /// </summary>
        [TestMethod]
        public void Query_For_TriangleGive_Point_Basic_Lists_WORK_IN_PROGRESS()
        {
            Random rnd = new Random(DateTime.Now.Second);
            var lstPoints = utils.Util.CreateRandomPoints(0, 100, CountOfPoints).ToList();
            var lstTriangles = utils.Util.FindAllTriangles(lstPoints).ToList();
            int[] idsOfPointsRandom = lstPoints.Select(p => p.ID).OrderBy(id => rnd.Next()).ToArray();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (int idOfPoint in idsOfPointsRandom)
            {
                var tri = lstTriangles.Where(t => t.Vertices.Any(tv => tv.Vertex.ID == idOfPoint)).ToArray();
                Assert.IsNotNull(tri);
                Assert.IsTrue(tri.Length > 0);
                //Assert.IsTrue(tri.Vertices.Any(tv => tv.Vertex.ID == idOfPoint));
            }
            sw.Stop();
            Trace.WriteLine($"Time taken to query for {idsOfPointsRandom.Length} points and '{lstTriangles.Count()}' triangles was {sw.ElapsedMilliseconds} ms ");

        }
        /// <summary>
        /// We are comparing the performance of Find() method vs doing a full scan through List<>
        /// </summary>
        [TestMethod]
        public void Query_For_Triangle_Using_PrimaryKey()
        {
            int countOfPoints = 50;
            ///
            /// Query for triangle using the Primary Key - using very basic List<>
            ///
            long timeTakenUsingBasicLists = 0;
            {
                Random rnd = new Random(DateTime.Now.Second);
                var lstPoints = utils.Util.CreateRandomPoints(0, 100, countOfPoints).ToList();
                var lstTriangles = utils.Util.FindAllTriangles(lstPoints).ToList();
                int[] idsOfTrianglesRandom = lstTriangles.Select(p => p.ID).OrderBy(id => rnd.Next()).ToArray();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (int idOfTriangle in idsOfTrianglesRandom)
                {
                    var tri = lstTriangles.Where(t => t.ID == idOfTriangle).ToArray();
                    Assert.IsTrue(tri.Length == 1);
                    Assert.AreEqual(idOfTriangle, tri[0].ID);
                }
                sw.Stop();
                timeTakenUsingBasicLists = sw.ElapsedMilliseconds;
                Trace.WriteLine($"List<>-Time taken to query for all '{lstTriangles.Count()}' triangles one by one was {timeTakenUsingBasicLists} ms ");
            }
            ///
            /// Query for triangle using the Primary key - using EF Find
            ///
            long timeTakenUsingEF = 0;
            {
                Random rnd = new Random(DateTime.Now.Second);
                var lstPoints = utils.Util.CreateRandomPoints(0, 100, countOfPoints).ToList();
                var lstTriangles = utils.Util.FindAllTriangles(lstPoints).ToList();
                int[] idsOfTrianglesRandom = lstTriangles.Select(p => p.ID).OrderBy(id => rnd.Next()).ToArray();
                DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
                ctx.Points.AddRange(lstPoints);
                ctx.Triangles.AddRange(lstTriangles);
                ctx.SaveChanges();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (int idOfTriangle in idsOfTrianglesRandom)
                {
                    var tri = ctx.Triangles.Find(idOfTriangle);
                    Assert.AreEqual(idOfTriangle, tri.ID);
                }
                sw.Stop();
                timeTakenUsingEF = sw.ElapsedMilliseconds;
                Trace.WriteLine($"SQLLITE-Time taken to query for all '{lstTriangles.Count()}' triangles one by one was {timeTakenUsingEF} ms ");
            }
            ///
            /// The time taken by EF-SQLLITE Find() should be significantly lower than that of raw List
            ///
            Assert.IsTrue(timeTakenUsingEF < timeTakenUsingBasicLists / 100);
        }
    }
}
