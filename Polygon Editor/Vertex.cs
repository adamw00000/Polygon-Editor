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
        public enum SideConstraint { Horizontal, Vertical, None }
        public enum VertexConstraint { Angle, None }

        public SideConstraint NextConstraint { get; set; }
        public SideConstraint PrevConstraint { get; set; }
        public VertexConstraint Constraint { get; set; }
        public bool IsConstraintSet => !(Constraint == VertexConstraint.None && PrevConstraint == SideConstraint.None && NextConstraint == SideConstraint.None);

        double angle;
        public double Angle
        {
            get
            {
                return angle * 180 / Math.PI;
            }
            set
            {
                angle = value * Math.PI / 180;
            }
        }
        public double aPrevConstraint { get; set; }
        public double aNextConstraint { get; set; }
        public double aPrev => Prev.X != X ? (Prev.Y - Y) / (Prev.X - X) : double.PositiveInfinity;
        public double aNext => Next.X != X ? (Next.Y - Y) / (Next.X - X) : double.PositiveInfinity;

        public double X { get; private set; }
        public double Y { get; private set; }
        public Vertex Next { get; set; }
        public Vertex Prev { get; set; }

        public Vertex(Point p, Vertex prev = null, Vertex next = null)
        {
            X = p.X;
            Y = p.Y;
            Prev = prev;
            Next = next;
            PrevConstraint = NextConstraint = SideConstraint.None;
            Constraint = VertexConstraint.None;
        }
        
        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public void Move(double x, double y)
        {
            X = x;
            Y = y;
            
            if (Constraint == VertexConstraint.Angle)
            {
                CorrectAngleThisVertex();
            }
            if (Next.Constraint == VertexConstraint.Angle)
            {
                Next.CorrectAngle();
            }
            if (Prev.Constraint == VertexConstraint.Angle)
            {
                Prev.CorrectAngle();
            }
        }

        public void EnforceConstraints(Vertex startVertex, Vertex endVertex)
        {
            switch (NextConstraint)
            {
                case SideConstraint.Horizontal:
                    if (Y != Next.Y)
                    {
                        Next.Move(Next.X, Y);
                    }
                    break;
                case SideConstraint.Vertical:
                    if (X != Next.X)
                    {
                        Next.Move(X, Next.Y);
                    }
                    break;
            }

            switch (PrevConstraint)
            {
                case SideConstraint.Horizontal:
                    if (Y != Prev.Y)
                    {
                        Prev.Move(Prev.X, Y);
                    }
                    break;
                case SideConstraint.Vertical:
                    if (X != Prev.X)
                    {
                        Prev.Move(X, Prev.Y);
                    }
                    break;
            }

            if (Constraint == VertexConstraint.Angle)
            {
                
            }

            if (this == endVertex)
                return;

            Next.EnforceConstraints(startVertex, endVertex);
        }

        public void CorrectAngle()
        {
            if (double.IsPositiveInfinity(aPrevConstraint) && double.IsPositiveInfinity(aNextConstraint))
            {
                return;
            }
            else if (double.IsPositiveInfinity(aPrevConstraint))
            {
                X = Prev.X;
                Y = aNextConstraint * (X - Next.X) + Next.Y;
                return;
            }
            else if (double.IsPositiveInfinity(aNextConstraint))
            {
                X = Next.X;
                Y = aPrevConstraint * (X - Prev.X) + Prev.Y;
                return;
            }

            X = (aPrevConstraint * Prev.X - Prev.Y - aNextConstraint * Next.X + Next.Y) / (aPrevConstraint - aNextConstraint);
            Y = aPrevConstraint * (X - Prev.X) + Prev.Y;
        }

        public void CorrectAngleThisVertex()
        {
            if (double.IsPositiveInfinity(aPrevConstraint) && double.IsPositiveInfinity(Prev.aPrev))
            {
            }
            else if (double.IsPositiveInfinity(aPrevConstraint))
            {
                Prev.X = X;
                Prev.Y = Prev.aPrev * (Prev.X - Prev.Prev.X) + Prev.Prev.Y;
            }
            else if (double.IsPositiveInfinity(Prev.aPrev))
            {
                Prev.X = Prev.Prev.X;
                Prev.Y = aPrevConstraint * (Prev.X - X) + Y;
            }
            else
            {
                Prev.X = (aPrevConstraint * X - Y - Prev.aPrev * Prev.Prev.X + Prev.Prev.Y) / (aPrevConstraint - Prev.aPrev);
                Prev.Y = aPrevConstraint * (Prev.X - X) + Y;
            }
            
            if (double.IsPositiveInfinity(aNextConstraint) && double.IsPositiveInfinity(Next.aNext))
            {
            }
            else if (double.IsPositiveInfinity(aNextConstraint))
            {
                Next.X = X;
                Next.Y = Next.aNext * (Next.X - Next.Next.X) + Next.Next.Y;
            }
            else if (double.IsPositiveInfinity(Next.aNext))
            {
                Next.X = Next.Next.X;
                Next.Y = aNextConstraint * (Next.X - X) + Y;
            }
            else
            {
                Next.X = (aNextConstraint * X - Y - Next.aNext * Next.Next.X + Next.Next.Y) / (aNextConstraint - Next.aNext);
                Next.Y = aNextConstraint * (Next.X - X) + Y;
            }
        }

        public double CalculateAngle()
        {
            double P12 = ToPoint().DistanceToPoint(Prev.ToPoint());
            double P23 = Next.ToPoint().DistanceToPoint(Prev.ToPoint());
            double P13 = ToPoint().DistanceToPoint(Next.ToPoint());

            double a = Math.Acos((P12 * P12 + P13 * P13 - P23 * P23) / (2 * P12 * P13));

            double xProduct = (Prev.X - X) * (Next.Y - Y) - (Next.X - X) * (Prev.Y - Y);
            if (xProduct < 0)
                a = -a;

            return a * 180 / Math.PI;
        }

        public void AddSideConstraint(SideConstraint sideConstraint)
        {
            if (NextConstraint != SideConstraint.None || Constraint != VertexConstraint.None || Next.Constraint != VertexConstraint.None)
            {
                MessageBox.Show("Too many constraints");
                return;
            }

            NextConstraint = sideConstraint;
            Next.PrevConstraint = sideConstraint;
        }

        public void AddVertexConstraint(VertexConstraint vertexConstraint)
        {
            if (NextConstraint != SideConstraint.None || PrevConstraint != SideConstraint.None || 
                Constraint != VertexConstraint.None || Prev.Constraint != VertexConstraint.None || Next.Constraint != VertexConstraint.None)
            {
                MessageBox.Show("Too many constraints");
                return;
            }

            Constraint = vertexConstraint;
            SetAngle();
            aNextConstraint = aNext;
            aPrevConstraint = aPrev;
        }

        private void SetAngle()
        {
            double prevAngle = Math.Atan2(Prev.Y - Y, Prev.X - X);
            double a = angle + prevAngle;

            double distance = ToPoint().DistanceToPoint(Next.ToPoint());
            Point deltaPos = new Point(distance * Math.Cos(a), distance * Math.Sin(a));
            Next.X = X + deltaPos.X;
            Next.Y = Y + deltaPos.Y;
        }

        public void ClearVertexConstraint()
        {
            Constraint = VertexConstraint.None;
        }

        public void ClearSideConstraint()
        {
            NextConstraint = SideConstraint.None;
            Next.PrevConstraint = SideConstraint.None;
        }
    }
}
