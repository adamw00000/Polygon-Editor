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
        public static double DistanceToPoint(this Point p, Point p2)
        {
            return Math.Sqrt((p.X - p2.X) * (p.X - p2.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
        }

        public static double DistanceToSegment(this Point p, double x1, double y1, double x2, double y2)
        {
            double x = p.X;
            double y = p.Y;

            double A = x - x1;
            double B = y - y1;
            double C = x2 - x1;
            double D = y2 - y1;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            double xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            double dx = x - xx;
            double dy = y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double DistanceToSegment(this Point p, Point p1, Point p2)
        {
            return DistanceToSegment(p, p1.X, p1.Y, p2.X, p2.Y);
        }
    }
}
