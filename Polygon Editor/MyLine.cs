using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Polygon_Editor
{
    class MyLine
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        double lastX1, lastX2, lastY1, lastY2;
        List<Point> points = new List<Point>();

        public List<Point> Points
        {
            get
            {
                Bresenham();
                return points;
            }
        }

        public MyLine(Point P1, Point P2)
        {
            X1 = P1.X;
            Y1 = P1.Y;
            X2 = P2.X;
            Y2 = P2.Y;
        }

        public void Bresenham()
        {
            if (lastX1 == X1 && lastY1 == Y1 && lastX2 == X2 && lastY2 == Y2)
                return;

            points.Clear();

            double tg = (Y2 - Y1) / (X2 - X1);
            if (tg >= -1 && tg <= 1)
            {
                if (X2 > X1)
                    BresenhamLowTg(X1, Y1, X2, Y2);
                else
                    BresenhamLowTg(X2, Y2, X1, Y1);

            }
            else
            {
                if (Y2 > Y1)
                    BresenhamHighTg(X1, Y1, X2, Y2);
                else
                    BresenhamHighTg(X2, Y2, X1, Y1);
            }

            lastX1 = X1;
            lastX2 = X2;
            lastY1 = Y1;
            lastY2 = Y2;
        }

        private void BresenhamLowTg(double x1, double y1, double x2, double y2)
        {
            int dx = (int)(x2 - x1);
            int dy = (int)(y2 - y1);
            int xf = (int)x1;
            int yf = (int)y1;
            int xb = (int)x2;
            int yb = (int)y2;
            int incrementY = 1;

            if (dy < 0)
            {
                dy = -dy;
                incrementY = -1;
            }

            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);
            int d = 2 * dy - dx;

            points.Add(new Point(xf, yf));
            points.Add(new Point(xb, yb));
            while (xf < xb)
            {
                xf++; xb--;
                if (d < 0)
                    d += incrE;
                else
                {
                    d += incrNE;
                    yf += incrementY;
                    yb -= incrementY;
                }
                points.Add(new Point(xf, yf));
                if (xf + 1 != xb)
                    points.Add(new Point(xb, yb));
            }
        }
        private void BresenhamHighTg(double x1, double y1, double x2, double y2)
        {
            int dx = (int)(x2 - x1);
            int dy = (int)(y2 - y1);
            int xf = (int)x1;
            int yf = (int)y1;
            int xb = (int)x2;
            int yb = (int)y2;
            int incrementX = 1;

            if (dx < 0)
            {
                dx = -dx;
                incrementX = -1;
            }

            int incrN = 2 * dx;
            int incrNE = 2 * (dx - dy);
            int d = 2 * dx - dy;

            points.Add(new Point(xf, yf));
            points.Add(new Point(xb, yb));
            while (yf < yb)
            {
                yf++; yb--;
                if (d < 0)
                    d += incrN;
                else
                {
                    d += incrNE;
                    xf += incrementX;
                    xb -= incrementX;
                }
                points.Add(new Point(xf, yf));
                if (yf + 1 != yb)
                    points.Add(new Point(xb, yb));
            }
        }
    }
}