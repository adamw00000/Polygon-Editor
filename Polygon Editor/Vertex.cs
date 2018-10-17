using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Polygon_Editor
{
    class Vertex
    {
        SideConstraint nextConstraint;
        SideConstraint prevConstraint;
        VertexConstraint constraint;

        public double X { get; set; }
        public double Y { get; set; }
        public Vertex Next { get; set; }
        public Vertex Prev { get; set; }

        public Vertex(Point p, Vertex prev = null, Vertex next = null)
        {
            X = p.X;
            Y = p.Y;
            Prev = prev;
            Next = next;
        }

        public Vertex(double x, double y, Vertex prev = null, Vertex next = null)
        {
            X = x;
            Y = y;
            Prev = prev;
            Next = next;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
