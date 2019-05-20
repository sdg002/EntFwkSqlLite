using DemoLib.entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            DemoLib.SqlDbContext ctx = CreateEF();

        }
        [TestMethod]
        public void SaveSinglePoint()
        {
            var pts = utils.Util.CreateRandomPoints(-1, 1, 1);
            Point[] ptsCloned = pts.Select(p => p.Clone()).ToArray();
            DemoLib.SqlDbContext ctx = CreateEF();
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
            DemoLib.SqlDbContext ctx = CreateEF();
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
            DemoLib.SqlDbContext ctx = CreateEF();
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
        private DemoLib.SqlDbContext CreateEF()
        {
            
            var connection = new SqliteConnection("DataSource=:memory:");
            var opts = new DbContextOptionsBuilder<DemoLib.SqlDbContext>()
                    .UseSqlite(connection)
                    .Options;
            connection.Open();
            var _dbctxInner = new DemoLib.SqlDbContext(opts);
            //_dbctxInner.Connection = connection;//We could add the Connection for a more unified Dispose management
            _dbctxInner.Database.EnsureCreated();
            return _dbctxInner;
        }

    }
}
