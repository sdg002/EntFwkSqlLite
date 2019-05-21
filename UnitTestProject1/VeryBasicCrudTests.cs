using DemoLib.entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
    /// <summary>
    /// The objective of the tests in this class is to ensure that we got the referential integrity right
    /// All objects that are saved should be retrived correctly
    /// </summary>
    [TestClass]
    public class VeryBasicCrudTests
    {
        [TestMethod]
        public void CreateDbContext()
        {
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            Assert.IsNotNull(ctx);
        }
        [TestMethod]
        public void SaveSinglePoint()
        {
            var pts = utils.Util.CreateRandomPoints(-1, 1, 1);
            Point[] ptsCloned = pts.Select(p => p.Clone()).ToArray();
            DemoLib.SqlDbContext ctx =utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.SaveChanges();
            Point[] ptsFromDb = ctx.Points.ToArray();
            Assert.IsTrue(ptsFromDb.Length == 1);
            Assert.AreEqual(ptsFromDb[0], ptsCloned[0]);
        }
        [TestMethod]
        public void SaveMultiplePoints()
        {
            int maxpoints = 10;
            var pts = utils.Util.CreateRandomPoints(-5, 5, maxpoints);
            Point[] ptsCloned = pts.Select(p => p.Clone()).ToArray();
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.SaveChanges();
            Point[] ptsFromDb = ctx.Points.ToArray();
            Assert.IsTrue(ptsFromDb.Length == maxpoints);
            for(int i=0;i<maxpoints;i++)
            {
                Assert.AreEqual(ptsFromDb[i], ptsCloned[i]);
            }
        }
        /// <summary>
        /// In this test we are testing for persisting 1 triangle
        /// </summary>
        [TestMethod]
        public void CreateOneTriangle()
        {
            var pts = utils.Util.CreateRandomPoints(-5, 5, 3);
            DemoLib.entity.Triangle tri = new Triangle();
            tri.Vertices.Add(new TriangleVertex { ParentID = tri.ID, Vertex = pts[0] });
            tri.Vertices.Add(new TriangleVertex { ParentID = tri.ID, Vertex = pts[1] });
            tri.Vertices.Add(new TriangleVertex { ParentID = tri.ID, Vertex = pts[2] });
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.Triangles.Add(tri);
            ctx.SaveChanges();
            Point[] ptsFromDb = ctx.Points.ToArray();
            var triFromDb = ctx.Triangles.First();
            Assert.AreEqual(triFromDb.Vertices.Count(), 3);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(ptsFromDb[i], triFromDb.Vertices.ElementAt(i).Vertex);
            }
        }
        /// <summary>
        /// In this test we are creating multiple triangles and saving them to db
        /// We are then testing if the retrieved triangles matched the original triangles
        /// This ensures that we got the foreign keys right
        /// </summary>
        [TestMethod]
        public void CreateMultipleTriangles()
        {
            int maxpoints = 4;
            var pts = utils.Util.CreateRandomPoints(-5, 5, maxpoints);
            Triangle[] triangles = utils.Util.FindAllTriangles(pts);
            List<int> idsOfVerticesOriginal = new List<int>();
            idsOfVerticesOriginal.AddRange(triangles.SelectMany(tri => tri.Vertices.Select(tv => tv.Vertex.ID)));
            Assert.AreEqual(4, triangles.Length);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.Triangles.AddRange (triangles);
            ctx.SaveChanges();
            Point[] ptsFromDb = ctx.Points.ToArray();
            Triangle[] trisFromDb = ctx.Triangles.ToArray();
            List<int> idsOfVerticesFromDb = new List<int>();
            idsOfVerticesFromDb.AddRange(trisFromDb.SelectMany(tri => tri.Vertices.Select(tv => tv.Vertex.ID)));
            CollectionAssert.AreEquivalent(idsOfVerticesFromDb, idsOfVerticesOriginal);
            for (int i=0;i<triangles.Length;i++)
            {
                Triangle triOriginal = triangles[i];
                Triangle triFromDb = trisFromDb[i];
                Assert.AreEqual(triFromDb.Vertices.Count(), 3);
                CollectionAssert.AreEquivalent(
                    triFromDb.Vertices.Select(tv => tv.Vertex).ToArray(), 
                    triOriginal.Vertices.Select(tv => tv.Vertex).ToArray());
            }
        }
        /// <summary>
        /// In this test we will ensure that a Point cannot be deleted if it is referenced by a Triangle
        /// </summary>
        [TestMethod]
        public void CascadeDeletePoint()
        {
            int maxpointsUsedForTriangles = 4,maxpointsNotUsedForTriangles=3;
            var ptsUsedForTriangles = utils.Util.CreateRandomPoints(-5, 5, maxpointsUsedForTriangles);
            var ptsNotUsedForTriangles = utils.Util.CreateRandomPoints(-5, 5, maxpointsNotUsedForTriangles);
            Triangle[] triangles = utils.Util.FindAllTriangles(ptsUsedForTriangles);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(ptsUsedForTriangles);
            ctx.Points.AddRange(ptsNotUsedForTriangles);
            ctx.Triangles.AddRange(triangles);
            ctx.SaveChanges();
            Point[] ptsFromDb = ctx.Points.ToArray();
            Assert.IsTrue(ptsFromDb.Length == maxpointsUsedForTriangles+maxpointsNotUsedForTriangles);
            Triangle[] trisFromDb = ctx.Triangles.ToArray();
            Assert.IsTrue(trisFromDb.Length== 4);
            ///
            /// Try to delete the points which are not being used for any triangles - should be deleted
            ///
            for(int i=0;i<ptsFromDb.Length;i++)
            {
                var pt = ptsFromDb[i];
                if (ptsUsedForTriangles.Any(p => p.ID == pt.ID)) continue;
                ctx.Points.Remove(pt);
                ctx.SaveChanges();
            }
            ///
            /// Try to delete the points which participate in triangles  - should not be allowed
            ///
            Point[] ptsFromDb2 = ctx.Points.ToArray();
            foreach (var pt in ptsFromDb2)
            {
                try
                {
                    ctx.Points.Remove(pt);
                    ctx.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    //OK - we expected this because of preventing cascade delete
                    Assert.IsTrue(ex.InnerException.Message.Contains("'FOREIGN KEY constraint failed"));
                }
                catch (Exception ex)
                {
                    Assert.Fail();
                }
            }
            Point[] ptsFromDb3 = ctx.Points.ToArray();
            Assert.AreEqual(maxpointsUsedForTriangles, ptsFromDb3.Length);
        }
        /// <summary>
        /// We are verifying that querying a Point using the primary key works
        /// </summary>
        [TestMethod]
        public void Query_Point_Using_PrimaryKey()
        {
            Random rnd = new Random(DateTime.Now.Second);
            int maxpoints = 4;
            var pts = utils.Util.CreateRandomPoints(-5, 5, maxpoints);
            Triangle[] triangles = utils.Util.FindAllTriangles(pts);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.Triangles.AddRange(triangles);
            ctx.SaveChanges();
            int[] idsOfPointsRandom = pts.Select(p => p.ID).OrderBy(id => rnd.Next()).ToArray();
            foreach (int idOfPoint in idsOfPointsRandom)
            {
                var point=ctx.Points.Find(idOfPoint);
                Assert.AreEqual(idOfPoint, point.ID);
            }
        }
        /// <summary>
        /// We are verifying that querying a Triangle using the primary key works
        /// </summary>
        [TestMethod]
        public void Query_Triangle_Using_PrimaryKey()
        {
            Random rnd = new Random(DateTime.Now.Second);
            int maxpoints = 4;
            var pts = utils.Util.CreateRandomPoints(-5, 5, maxpoints);
            Triangle[] triangles = utils.Util.FindAllTriangles(pts);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.Triangles.AddRange(triangles);
            ctx.SaveChanges();
            int[] idsOfTrianglesRandom = triangles.
                                        Select(tri => tri.ID).
                                        OrderBy(id => rnd.Next()).ToArray();
            foreach (int idOfTriangle in idsOfTrianglesRandom)
            {
                var tri = ctx.Triangles.Find(idOfTriangle);
                Assert.AreEqual(idOfTriangle, tri.ID);
            }
        }
        [TestMethod]
        public void QueryForTriangle_Given_Point()
        {
            Random rnd = new Random(DateTime.Now.Second);
            int maxpoints = 4;
            var pts = utils.Util.CreateRandomPoints(-5, 5, maxpoints);
            Triangle[] triangles = utils.Util.FindAllTriangles(pts);
            DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
            ctx.Points.AddRange(pts);
            ctx.Triangles.AddRange(triangles);
            ctx.SaveChanges();
            ///
            /// We have the database ready, now iterate over every point and verify that a LINQ query works
            ///
            int[] idsOfPointsRandom = pts.Select(p => p.ID).OrderBy(id => rnd.Next()).ToArray();
            foreach(int idOfPoint in idsOfPointsRandom)
            {
                var tri=ctx.Triangles.Where(t => t.Vertices.Any(tv => tv.Vertex.ID == idOfPoint)).FirstOrDefault();
                Assert.IsNotNull(tri);
                Assert.IsTrue(tri.Vertices.Any(tv => tv.PointID == idOfPoint));
            }
        }
        ///// <summary>
        ///// In this test we are checking whether FK integrity is working.
        ///// We will attempt to add a Triangle using Points which have not been added to the database
        ///// </summary>
        //[TestMethod]
        //public void Adding_Trianlge_With_NonExistent_Point_Should_Not_Be_Allowed()
        //{
        //    try
        //    {
        //        int maxpoints = 4;
        //        var pts = utils.Util.CreateRandomPoints(-5, 5, maxpoints);
        //        Triangle[] triangles = utils.Util.FindAllTriangles(pts);
        //        DemoLib.SqlDbContext ctx = utils.Util.CreateSqlLiteContext();
        //        ctx.Triangles.AddRange(triangles);//Add triangles without adding Points
        //        ctx.SaveChanges();
        //        var pts1=ctx.Points.ToArray();
        //        Assert.Fail("Exception was expected");
        //          This did not work as expected - the points get Saved too when saving triangles
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        //OK
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("The right exception was not thrown");
        //    }
        //}
    }
}
