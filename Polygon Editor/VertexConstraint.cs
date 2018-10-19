//using System;
//using System.Windows;

//namespace Polygon_Editor
//{
//    internal interface IVertexConstraint
//    {
//        void Enforce(Vertex v);
//        bool Check(Vertex v);
//    }

//    class AngleVertexConstraint: IVertexConstraint
//    {
//        public double Angle { get; set; }

//        public AngleVertexConstraint(double angle)
//        {
//            Angle = Math.PI * Angle / 180;
//        }

//        public void Enforce(Vertex v)
//        {
//            //double prevAngle = Math.Atan2(v.Prev.Y - v.Y, v.Prev.X - v.X);
//            //double angle = Math.PI * Angle / 180 + prevAngle;

//            //double distance = v.ToPoint().DistanceToPoint(v.Next.ToPoint());
//            //Point deltaPos = new Point(distance * Math.Cos(angle),
//            //    distance * Math.Sin(angle));
//            //v.Next.X = v.X + deltaPos.X;
//            //v.Next.Y = v.Y + deltaPos.Y;
//            Vector delta = Point.Subtract(v.Next.ToPoint(), v.Prev.ToPoint()) / 2;
//            double tan = Math.Tan(Angle / 2);
//            Vector x = delta / tan;
//            delta.X += -x.Y;
//            delta.Y += x.X;
//            v.X = v.Prev.X + delta.X;
//            v.Y = v.Prev.Y + delta.Y;
//        }

//        public bool Check(Vertex v)
//        {
//            double P12 = v.ToPoint().DistanceToPoint(v.Prev.ToPoint());
//            double P23 = v.Next.ToPoint().DistanceToPoint(v.Prev.ToPoint());
//            double P13 = v.ToPoint().DistanceToPoint(v.Next.ToPoint());

//            double a = Math.Acos((P12 * P12 + P13 * P13 - P23 * P23) / (2 * P12 * P13));
//            double eps = 0.1;
//            return (Math.Abs(a - Angle) < eps);
//        }
//    }
//}