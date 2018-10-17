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

    //class Vertex
    //{
    //    Ellipse dot = new Ellipse
    //    {
    //        Stroke = new SolidColorBrush(Colors.Black),
    //        StrokeThickness = 3, 
    //        Height = Constants.PointSize,
    //        Width = Constants.PointSize,
    //        Fill = new SolidColorBrush(Colors.Black),
    //        Margin = new Thickness(0, 0, 0, 0)
    //};

    //    public Point P { get; set; }
    //    public Ellipse Dot
    //    {
    //        get
    //        {
    //            int size = Constants.PointSize;
    //            dot.Margin = new Thickness(P.X - size / 2, P.Y - size / 2, 0, 0);
    //            return dot;
    //        }
    //    }

    //    public Vertex(Point P)
    //    {
    //        this.P = P;
    //    }
    //}

    //class Side
    //{
    //    MyLine line;

    //    public Vertex V1 { get; set; }
    //    public Point Middle => new Point((V1.P.X + V2.P.X) / 2, (V1.P.Y + V2.P.Y) / 2);
    //    public Vertex V2 { get; set; }
    //    public MyLine Line
    //    {
    //        get
    //        {
    //            line.X1 = V1.P.X;
    //            line.Y1 = V1.P.Y;
    //            line.X2 = V2.P.X;
    //            line.Y2 = V2.P.Y;
    //            return line;
    //        }
    //    }

    //    public Side(Vertex V1, Vertex V2)
    //    {
    //        this.V1 = V1;
    //        this.V2 = V2;
    //        line = new MyLine(V1.P, V2.P); 
    //    }
    //}

    class Polygon
    {
        public CyclicBidirectionalVerticesList Vertices { get; set; } = new CyclicBidirectionalVerticesList();

        internal void MoveVertex(Vertex movedVertex, Point p)
        {
            movedVertex.X = p.X;
            movedVertex.Y = p.Y;
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
