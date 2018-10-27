using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

    [DataContract]
    class Polygon: ICloneable
    {
        [DataMember]
        public CyclicBidirectionalVerticesList Vertices { get; set; } = new CyclicBidirectionalVerticesList();
        public Polygon clone;
        int boundsX;
        int boundsY;

        public Polygon(bool toClone = true)
        {
            if (toClone)
                clone = (Polygon)Clone();
        }

        public void SetBounds(int x, int y)
        {
            boundsX = x;
            boundsY = y;
        }

        internal bool MoveVertex(Vertex v, Point p)
        {
            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            vertex.Move(p.X, p.Y);
            vertex.EnforceConstraints(vertex, vertex.Prev);

            if (!CheckClonePositions())
                return false;
            if (!CheckCloneConstraints(vertex))
                return false;

            v.Move(p.X, p.Y);
            v.EnforceConstraints(v, v.Prev);
            return true;
        }

        internal bool CheckClonePositions()
        {
            foreach (var ver in clone.Vertices.Enumerate())
                if (ver.X > boundsX || ver.Y > boundsY || ver.X < 0 || ver.Y < 0)
                {
                    clone = (Polygon)Clone();
                    MessageBox.Show("Cannot move vertex out of bounds");
                    return false;
                }
            return true;
        }

        internal bool CheckCloneConstraints(Vertex vertex)
        {
            if (!vertex.CheckConstraints(vertex.Prev))
            {
                MessageBox.Show("Cannot apply constraints (in case of moving might be a result of numerical error)");
                clone = (Polygon)Clone();
                return false;
            }
            return true;
        }

        internal void AddVertex(Vertex v)
        {
            clone.Vertices.AddVertex((Vertex)v.Clone());
            Vertices.AddVertex(v);
        }

        internal void AddVertexAfter(Vertex v)
        {
            Point location = new Point((v.X + v.Next.X) / 2, (v.Y + v.Next.Y) / 2);
            
            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            clone.Vertices.AddAfter(new Vertex(location), vertex);

            Vertices.AddAfter(new Vertex(location), v);
        }

        internal void RemoveVertex(Vertex v)
        {
            if (Vertices.Count <= 3)
                return;

            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            clone.Vertices.RemoveVertex(vertex);
            Vertices.RemoveVertex(v);
        }

        public void AddAngleConstraint(Vertex v, double angle, Vertex.VertexConstraint vertexConstraint)
        {
            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            if (!vertex.AddAngleConstraint(angle))
                return;
            vertex.EnforceConstraints(vertex, vertex.Prev);
            
            if (!CheckClonePositions())
                return;
            if (!CheckCloneConstraints(vertex))
                return;

            v.Angle = angle;
            v.AddAngleConstraint(angle);
            v.EnforceConstraints(v, v.Prev);
        }

        public void AddSideConstraint(Vertex v, Vertex.SideConstraint sideConstraint)
        {
            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            if (!vertex.AddSideConstraint(sideConstraint))
                return;
            vertex.EnforceConstraints(vertex, vertex.Prev);

            if (!CheckClonePositions())
                return;
            if (!CheckCloneConstraints(vertex))
                return;

            v.AddSideConstraint(sideConstraint);
            v.EnforceConstraints(v, v.Prev);
        }

        public void ClearVertexConstraint(Vertex v)
        {
            v.ClearVertexConstraint();

            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            vertex.ClearVertexConstraint();
        }

        public void ClearSideConstraint(Vertex v)
        {
            v.ClearSideConstraint();

            Vertex vertex = clone.Vertices.GetVertexAt(Vertices.GetIndex(v));
            vertex.ClearSideConstraint();
        }

        public Vertex GetClickedVertex(Point p)
        {
            return Vertices.GetClickedVertex(p);
        }

        internal Vertex GetClickedSideFirstVertex(Point p)
        {
            return Vertices.GetClickedSideFirstVertex(p);
        }

        public object Clone()
        {
            var list = new CyclicBidirectionalVerticesList();
            foreach (var v in Vertices.Enumerate())
                list.AddVertex((Vertex)v.Clone());
            return new Polygon(false) { Vertices = list };
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            clone = (Polygon)Clone();
        }
    }
}
