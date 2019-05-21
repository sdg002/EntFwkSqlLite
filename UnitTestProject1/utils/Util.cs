using DemoLib.entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnitTestProject1.utils
{
    /// <summary>
    /// Methods to help unit testing
    /// </summary>
    public static class Util
    {
        static Random _rnd = new Random(DateTime.Now.Second);
        /// <summary>
        /// Returns the path to the current unit testing project. This method uses the path of the executing assembly and traverses upwards
        /// </summary>
        /// <returns></returns>
        public static string GetProjectDir2()
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = System.IO.Path.GetDirectoryName(pathAssembly);
            if (folderAssembly.EndsWith("\\") == false) folderAssembly = folderAssembly + "\\";
            string folderProjectLevel = System.IO.Path.GetFullPath(folderAssembly + "..\\..\\");
            Trace.WriteLine($"Project directory:{folderProjectLevel}");
            return folderProjectLevel;
        }
        /// <summary>
        /// Generates the specified no of random points within the limits
        /// </summary>
        /// <param name="min">X,Y of the one end of the diagonal</param>
        /// <param name="max">X,Y of the opposite end of the diagonal</param>
        /// <param name="maxcount">Total no of points to create</param>
        /// <returns></returns>
        internal static Point[] CreateRandomPoints(double min, double max, int maxcount)
        {
            List<Point> pts = new List<Point>();
            double range = max - min;
            for (int index = 0; index < maxcount; index++)
            {
                double x = _rnd.NextDouble() * range + min;
                double y = _rnd.NextDouble() * range + min;
                Point p = new Point(x, y);
                pts.Add(p);
            }
            return pts.ToArray();
        }
        /// <summary>
        /// Given N points, this function will find all unique combinations of 3 points
        /// </summary>
        /// <param name="points">Points</param>
        /// <returns></returns>
        public static Triangle[] FindAllTriangles(IEnumerable<Point> pts)
        {
            Point[] points = pts.ToArray();
            //you were here//How, you cannot use a for loop, 
            //create an extension method to generate pairs of items from an collection
            //have an outerloop, loop over every item, pre-generate pairs for the rest, skip where pair contains the outer item
            //you were here.

            List<Point[]> three_grams = new List<Point[]>();
            for (int i = 0; i < points.Length; i++)
            {
                Point pt_1_of_3 = points[i];
                for (int j = i + 1; j < points.Length; j++)
                {
                    Point pt_2_of_3 = points[j];
                    for (int k = j + 1; k < points.Length; k++)
                    {
                        Point pt_3_of_3 = points[k];
                        var arr3gram = new Point[]
                        {
                            pt_1_of_3,
                            pt_2_of_3,
                            pt_3_of_3
                        };
                        three_grams.Add(arr3gram);
                    }
                }
            }
            List<Triangle> lstTriangles = new List<Triangle>();
            foreach (Point[] vertices in three_grams)
            {
                Triangle tri = CreateTriangleFrom3Points(vertices);
                //tri.Vertices = vertices; //This breaks EF Core. Colleciton cannot be an Array if it is to be managed by EF.
                lstTriangles.Add(tri);
            }
            return lstTriangles.ToArray();
        }
        /// <summary>
        /// Creates a single Triangle object with the specified 3 vertices
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private static Triangle CreateTriangleFrom3Points(Point[] vertices)
        {
            if (vertices.Length != 3) throw new ArgumentException("3 vertices expected");
            DemoLib.entity.Triangle tri = new Triangle();
            tri.Vertices.Add(new TriangleVertex { ParentID = tri.ID, Vertex = vertices[0] });
            tri.Vertices.Add(new TriangleVertex { ParentID = tri.ID, Vertex = vertices[1] });
            tri.Vertices.Add(new TriangleVertex { ParentID = tri.ID, Vertex = vertices[2] });
            return tri;
        }
        /// <summary>
        /// Single creation method, responsible for creating the Db context with right parameters
        /// </summary>
        /// <returns></returns>
        internal static DemoLib.SqlDbContext CreateSqlLiteContext()
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
