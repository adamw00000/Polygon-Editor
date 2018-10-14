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
        public Point P { get; set; }
        public Ellipse Dot { get; set; }

        public Vertex(Point P, Ellipse Dot)
        {
            this.P = P;
            this.Dot = Dot;
        }

        public Ellipse CreateDot()
        {
            int size = Constants.PointSize;
            Ellipse currentDot = new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 3
            };
            currentDot.Height = size;
            currentDot.Width = size;
            currentDot.Fill = new SolidColorBrush(Colors.Black);
            currentDot.Margin = new Thickness(P.X - size / 2, P.Y - size / 2, 0, 0);
            Dot = currentDot;
            return currentDot;
        }
    }

    class Side
    {
        public Vertex V1 { get; set; }
        public Point Middle { get; set; }
        public Vertex V2 { get; set; }
        public Line Line { get; set; }

        public Side(Vertex V1, Point Middle, Vertex V2, Line Line)
        {
            this.V1 = V1;
            this.Middle = Middle;
            this.V2 = V2;
            this.Line = Line;
        }

        public Point CalculateMiddle() => new Point((V1.P.X + V2.P.X) / 2, (V1.P.Y + V2.P.Y) / 2);

        public Line CreateLine() => new Line()
        {
            X1 = V1.P.X,
            Y1 = V1.P.Y,
            X2 = V2.P.X,
            Y2 = V2.P.Y,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };
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

            var sides = Sides.FindAll(side => side.V1 == movedVertex || side.V2 == movedVertex);
            foreach(var side in sides)
            {
                side.Line.X1 = side.V1.P.X;
                side.Line.Y1 = side.V1.P.Y;
                side.Line.X2 = side.V2.P.X;
                side.Line.Y2 = side.V2.P.Y;
                side.Middle = new Point((side.V1.P.X + side.V2.P.X) / 2, (side.V1.P.Y + side.V2.P.Y) / 2);
            }
        }

        internal void AddVertex(Side side)
        {
            Point p = side.Middle;
            Vertex v = new Vertex(p, null);
            v.CreateDot();
            Vertexes.Add(v);

            Side s1 = new Side(side.V1, new Point(), v, null);
            s1.Line = s1.CreateLine();
            s1.Middle = s1.CalculateMiddle();
            Side s2 = new Side(v, new Point(), side.V2, null);
            s2.Line = s2.CreateLine();
            s2.Middle = s2.CalculateMiddle();

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
            Side newSide = new Side(V1, middle, V2, new Line()
            {
                X1 = V1.P.X,
                Y1 = V1.P.Y,
                X2 = V2.P.X,
                Y2 = V2.P.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            });
            Sides.RemoveAt(sideIndex);
            Sides.RemoveAt(secondSideIndex == 0 ? 0 : secondSideIndex - 1);
            Sides.Insert(secondSideIndex == 0 ? 0 : sideIndex, newSide);
        }
    }
}
