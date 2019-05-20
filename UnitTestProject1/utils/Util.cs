using DemoLib.entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    }
}
