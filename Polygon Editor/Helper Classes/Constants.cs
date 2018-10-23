using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Polygon_Editor
{
    public static class Constants
    {
        public const int PointSize = 12;
        //public const int MiddlePointSize = 6;
        public const int ClickEps = 5;
        public const double Eps = 0.01;
        public static BitmapImage HorizontalImage = 
            new BitmapImage(new Uri("Icons/horizontal.png", UriKind.Relative));
        public static BitmapImage VerticalImage =
            new BitmapImage(new Uri("Icons/vertical.png", UriKind.Relative));
        public static BitmapImage AngleImage =
            new BitmapImage(new Uri("Icons/angle.png", UriKind.Relative));
    }
}
