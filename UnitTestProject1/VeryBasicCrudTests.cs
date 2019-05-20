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
