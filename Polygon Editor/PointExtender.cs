using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Polygon_Editor
{
    static class PointExtender
    {
        public static double DistanceTo(this Point p, Point p2)
        {
            return Math.Sqrt((p.X - p2.X) * (p.X - p2.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
        }
    }
}
