using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Polygon_Editor
{
    class CyclicBidirectionalVerticesList
    {
        Vertex head;
        Vertex tail;
        public int Count { get; set; }

        public void AddVertex(Vertex v)
        {
            if (Count == 0)
            {
                head = v;
                tail = v;
                v.Next = v.Prev = v;
            }
            else
            {
                v.Prev = tail;
                v.Next = tail.Next;
                tail.Next = v;
                head.Prev = v;
                tail = v;
            }

            Count++;
        }

        public void RemoveVertex(Vertex v)
        {
            if (v.Next == null || v.Prev == null)
                return;
            if (Count == 1)
            {
                head = tail = null;
            }
            else
            {
                if (v == head)
                    head = head.Next;
                if (v == tail)
                    tail = v.Prev;
                v.Prev.Next = v.Next;
                v.Next.Prev = v.Prev;
            }

            Count--;
        }

        public void AddAfter(Vertex v, Vertex prev)
        {
            v.Prev = prev;
            v.Next = prev.Next;
            prev.Next.Prev = v;
            prev.Next = v;

            if (prev == tail)
                tail = v;

            v.Prev?.ClearNextConstaint();
            v.Next?.ClearPrevConstaint();

            Count++;
        }

        public IEnumerable<Vertex> Enumerate()
        {
            Vertex v = head;
            for (int i = 0; i < Count; i++)
            {
                yield return v;
                v = v.Next;
            }

        }

        internal Vertex GetClickedVertex(Point p)
        {
            foreach (var v in Enumerate())
            {
                if (v.ToPoint().DistanceToPoint(p) <= Constants.PointSize + Constants.ClickEps)
                    return v;
            }
            return null;
        }

        internal Vertex GetClickedSideFirstVertex(Point p)
        {
            foreach (var v in Enumerate())
            {
                if (p.DistanceToSegment(v.ToPoint(), v.Next.ToPoint()) <= Constants.ClickEps)
                    return v;
            }
            return null;
        }
    }
}
