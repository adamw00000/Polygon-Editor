using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Polygon_Editor
{
    public static class WriteableBitmapExtender
    {
        public static void Bresenham(this WriteableBitmap bitmap, double X1, double Y1, double X2, double Y2)
        {
            double tg = (Y2 - Y1) / (X2 - X1);
            if (tg >= -1 && tg <= 1)
            {
                if (X2 > X1)
                    BresenhamLowTg(bitmap, X1, Y1, X2, Y2);
                else
                    BresenhamLowTg(bitmap, X2, Y2, X1, Y1);

            }
            else
            {
                if (Y2 > Y1)
                    BresenhamHighTg(bitmap, X1, Y1, X2, Y2);
                else
                    BresenhamHighTg(bitmap, X2, Y2, X1, Y1);
            }
        }

        private static void BresenhamLowTg(WriteableBitmap bitmap, double x1, double y1, double x2, double y2)
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

            bitmap.SetPixel(xf, yf, Colors.Black);
            bitmap.SetPixel(xb, yb, Colors.Black);
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
                bitmap.SetPixel(xf, yf, Colors.Black);
                if (xf + 1 != xb)
                    bitmap.SetPixel(xb, yb, Colors.Black);
            }
        }
        private static void BresenhamHighTg(WriteableBitmap bitmap, double x1, double y1, double x2, double y2)
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

            bitmap.SetPixel(xf, yf, Colors.Black);
            bitmap.SetPixel(xb, yb, Colors.Black);
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
                bitmap.SetPixel(xf, yf, Colors.Black);
                if (yf + 1 != yb)
                    bitmap.SetPixel(xb, yb, Colors.Black);
            }
        }
    }
}
