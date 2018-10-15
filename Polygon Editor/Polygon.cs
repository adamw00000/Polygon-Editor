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

    class Vertex
    {
        Ellipse dot = new Ellipse
        {
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 3, 
            Height = Constants.PointSize,
            Width = Constants.PointSize,
            Fill = new SolidColorBrush(Colors.Black),
            Margin = new Thickness(0, 0, 0, 0)
    };

        public Point P { get; set; }
        public Ellipse Dot
        {
            get
            {
                int size = Constants.PointSize;
                dot.Margin = new Thickness(P.X - size / 2, P.Y - size / 2, 0, 0);
                return dot;
            }
        }

        public Vertex(Point P)
        {
            this.P = P;
        }
    }

    class Side
    {
        MyLine line;

        public Vertex V1 { get; set; }
        public Point Middle => new Point((V1.P.X + V2.P.X) / 2, (V1.P.Y + V2.P.Y) / 2);
        public Vertex V2 { get; set; }
        public MyLine Line
        {
            get
            {
                line.X1 = V1.P.X;
                line.Y1 = V1.P.Y;
                line.X2 = V2.P.X;
                line.Y2 = V2.P.Y;
                return line;
            }
        }

        public Side(Vertex V1, Vertex V2)
        {
            this.V1 = V1;
            this.V2 = V2;
            line = new MyLine(V1.P, V2.P); 
        }
    }

    class Polygon
    {
        public List<Vertex> Vertexes { get; set; } = new List<Vertex>();
        public List<Side> Sides { get; set; } = new List<Side>();

        public ClickTarget CheckClickTarget(Point p, out object target)
        {
            if (Vertexes.Count > 0 && Vertexes[0].P.DistanceTo(p) <= Constants.PointSize)
            {
                target = Vertexes[0];
                return ClickTarget.FirstVertex;
            }

            var vertexMatch = Vertexes.Find(point => point.P.DistanceTo(p) <= Constants.PointSize);

            if (vertexMatch != null)
            {
                target = vertexMatch;
                return ClickTarget.Vertex;
            }

            var sideMatch = Sides.Find(side => side.Middle.DistanceTo(p) <= Constants.MiddlePointSize);

            if (sideMatch != null)
            {
                target = sideMatch;
                return ClickTarget.Side;
            }

            target = null;
            return ClickTarget.None;
        }

        internal void MoveVertex(Vertex movedVertex, Point p)
        {
            movedVertex.P = p;
            double size = Constants.PointSize;
            movedVertex.Dot.Margin = new Thickness(p.X - size / 2, p.Y - size / 2, 0, 0);
        }

        internal void AddVertex(Side side)
        {
            Point p = side.Middle;
            Vertex v = new Vertex(p);
            Vertexes.Add(v);

            Side s1 = new Side(side.V1, v);
            Side s2 = new Side(v, side.V2);

            int i = Sides.FindIndex(s => s == side);
            Sides.Insert(i, s1);
            Sides.Insert(i + 1, s2);
            Sides.RemoveAt(i + 2);
        }

        internal void RemoveVertex(Vertex vertex)
        {
            if (Vertexes.Count <= 3)
                return;
            Vertexes.Remove(vertex);

            int sideIndex = Sides.FindIndex(side => side.V2 == vertex);
            int secondSideIndex = (sideIndex + 1) % Sides.Count;
            Vertex V1 = Sides[sideIndex].V1;
            Vertex V2 = Sides[secondSideIndex].V2;

            Point middle = new Point((V1.P.X + V2.P.X) / 2, (V1.P.Y + V2.P.Y) / 2);
            Side newSide = new Side(V1, V2);
            Sides.RemoveAt(sideIndex);
            Sides.RemoveAt(secondSideIndex == 0 ? 0 : secondSideIndex - 1);
            Sides.Insert(secondSideIndex == 0 ? 0 : sideIndex, newSide);
        }
    }
}
