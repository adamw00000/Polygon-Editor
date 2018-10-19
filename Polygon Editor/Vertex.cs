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
        //bool isConstraintSet = false;

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
            PrevConstraint = NextConstraint = SideConstraint.None;
            Constraint = VertexConstraint.None;
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

        public void Move(double x, double y)
        {
            X = x;
            Y = y;
            EnforceConstraints();
        }

        public void EnforceConstraints()
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
                if (Math.Abs(Angle - CalculateAngle()) > Constants.Eps)
                {
                    if (PrevConstraint != SideConstraint.None)
                        CorrectAngle(Prev, this, Next);
                    else
                        CorrectAngle(Next, this, Prev);

                    //Vector delta = Point.Subtract(Next.ToPoint(), Prev.ToPoint()) / 2;
                    //double tan = Math.Tan(Angle / 2);
                    //Vector x = delta / tan;
                    //delta.X += -x.Y;
                    //delta.Y += x.X;
                    //X = Prev.X + delta.X;
                    //Y = Prev.Y + delta.Y;
                }
            }

            if (Next.Constraint == VertexConstraint.Angle && 
                Math.Abs(Next.CalculateAngle() - Next.Angle) >= Constants.Eps)
                Next.EnforceConstraints();
            if (Prev.Constraint == VertexConstraint.Angle &&
                Math.Abs(Prev.CalculateAngle() - Prev.Angle) >= Constants.Eps)
                Prev.EnforceConstraints();
        }

        private void CorrectAngle(Vertex prev, Vertex v, Vertex next)
        {
            double prevAngle = Math.Atan2(prev.Y - v.Y, prev.X - v.X);
            double a = angle + prevAngle;

            double distance = v.ToPoint().DistanceToPoint(next.ToPoint());
            Point deltaPos = new Point(distance * Math.Cos(a),
                distance * Math.Sin(a));
            next.Move(v.X + deltaPos.X, v.Y + deltaPos.Y);
            next.EnforceConstraints();
        }

        public double CalculateAngle()
        {
            double P12 = ToPoint().DistanceToPoint(Prev.ToPoint());
            double P23 = Next.ToPoint().DistanceToPoint(Prev.ToPoint());
            double P13 = ToPoint().DistanceToPoint(Next.ToPoint());

            double a = Math.Acos((P12 * P12 + P13 * P13 - P23 * P23) / (2 * P12 * P13));

            if ((Prev.X - X) * (Next.Y - Y) - (Next.X - X) * (Prev.Y - Y) > 0)
                a = -a;

            return a * 180 / Math.PI;
        }

        public void AddSideConstraint(SideConstraint sideConstraint)
        {
            NextConstraint = sideConstraint;
            Next.PrevConstraint = sideConstraint;
        }

        public void AddVertexConstraint(VertexConstraint vertexConstraint)
        {
            Constraint = vertexConstraint;
        }

        public void ClearNextConstaint()
        {
            NextConstraint = SideConstraint.None;
        }

        public void ClearPrevConstaint()
        {
            PrevConstraint = SideConstraint.None;
        }
    }
}
