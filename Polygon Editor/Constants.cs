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
        public const int MiddlePointSize = 6;
        public const int ClickEps = 5;
        public static BitmapImage NoConstraintImage = 
            new BitmapImage(new Uri("Icons/noconstraint.png", UriKind.Relative));
    }
}
