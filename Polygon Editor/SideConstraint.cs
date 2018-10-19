//namespace Polygon_Editor
//{
//    internal interface ISideConstraint
//    {
//        void Enforce(Vertex v1, Vertex v2);
//        bool Check(Vertex v1, Vertex v2);
//    }

//    class HorizontalSideConstraint : ISideConstraint
//    {
//        public bool Check(Vertex v1, Vertex v2)
//        {
//            return v1.Y == v2.Y;
//        }

//        public void Enforce(Vertex v1, Vertex v2)
//        {
//            v1.Y = v2.Y;
//        }
//    }

//    class VerticalSideConstraint : ISideConstraint
//    {
//        public bool Check(Vertex v1, Vertex v2)
//        {
//            return v1.X == v2.X;
//        }

//        public void Enforce(Vertex v1, Vertex v2)
//        {
//            v1.X = v2.X;
//        }
//    }
//}