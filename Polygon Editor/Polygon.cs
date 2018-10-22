using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Polygon_Editor
{
    enum ClickTarget
    {
        None, FirstVertex, Vertex, Side
    }

    class Polygon
    {
        public CyclicBidirectionalVerticesList Vertices { get; set; } = new CyclicBidirectionalVerticesList();

        internal void MoveVertex(Vertex v, Point p)
        {
            v.Move(p.X, p.Y);
            v.EnforceConstraints(v, v.Prev);
        }

        internal void AddVertex(Vertex v)
        {
            Vertices.AddVertex(v);
        }

        internal void AddVertexAfter(Vertex v)
        {
            Point location = new Point((v.X + v.Next.X) / 2, (v.Y + v.Next.Y) / 2);
            Vertices.AddAfter(new Vertex(location), v);
        }

        internal void RemoveVertex(Vertex vertex)
        {
            if (Vertices.Count <= 3)
                return;
            Vertices.RemoveVertex(vertex);
        }

        public Vertex GetClickedVertex(Point p)
        {
            return Vertices.GetClickedVertex(p);
        }

        internal Vertex GetClickedSideFirstVertex(Point p)
        {
            return Vertices.GetClickedSideFirstVertex(p);
        }
    }
}
