using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Polygon_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polygon polygon = new Polygon();

        Vertex lastVertex = null;

        bool drawMode = true;
        bool moveMode = false;
        Vertex movedVertex;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Title = "DOWN";
            var p = e.GetPosition(this);

            if (drawMode)
            {
                var result = polygon.CheckClickTarget(p, out object vertex);

                if (result == ClickTarget.FirstVertex)
                {
                    DrawSide(lastVertex, ((Vertex)vertex));
                    drawMode = false;
                    return;
                }

                Ellipse dot = CreateDot(p, Constants.PointSize);
                DrawDot(dot);
                Vertex v = new Vertex(p);
                polygon.Vertexes.Add(v);

                if (lastVertex != null)
                {
                    DrawSide(lastVertex, v);
                }

                lastVertex = v;
            }
            else
            {
                var result = polygon.CheckClickTarget(p, out object vertex);

                if (result == ClickTarget.Vertex || result == ClickTarget.FirstVertex)
                {
                    moveMode = true;
                    movedVertex = (Vertex)vertex;
                }
                else if (polygon.CheckClickTarget(p, out object side) == ClickTarget.Side)
                {
                    Side clickedSide = (Side)side;
                    polygon.AddVertex(clickedSide);
                    DrawPolygon();
                }
            }
        }

        private void DrawSide(Vertex v1, Vertex v2)
        {
            Point p1 = v1.P, p2 = v2.P;

            Side s = new Side(v1, v2);
            DrawLine(s.Line);
            Ellipse dot = CreateDot(s.Middle, Constants.MiddlePointSize);
            DrawDot(dot);

            polygon.Sides.Add(s);
        }

        private void DrawLine(MyLine line)
        {
            line.Bresenham();
            foreach (var point in line.Points)
                Canvas.Children.Add(CreateDot(point, 1));
        }

        private void DrawDot(Ellipse dot)
        {
            Canvas.Children.Add(dot);
        }

        private static Ellipse CreateDot(Point p, int size)
        {
            Ellipse currentDot = new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 3
            };
            currentDot.Height = size;
            currentDot.Width = size;
            currentDot.Fill = new SolidColorBrush(Colors.Black);
            currentDot.Margin = new Thickness(p.X - size / 2, p.Y - size / 2, 0, 0);
            return currentDot;
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);

            if (!drawMode)
            {
                if (polygon.CheckClickTarget(p, out object vertex) == ClickTarget.Vertex ||
                    polygon.CheckClickTarget(p, out vertex) == ClickTarget.FirstVertex)
                {
                    polygon.RemoveVertex((Vertex)vertex);
                    DrawPolygon();
                }
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Title = "UP";
            var p = e.GetPosition(this);
            
            if (moveMode)
            {
                moveMode = false;
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }

        private void DrawPolygon()
        {
            Canvas.Children.Clear();
            foreach (var side in polygon.Sides)
            {
                DrawLine(side.Line);
                DrawDot(CreateDot(side.Middle, Constants.MiddlePointSize));
                DrawDot(side.V2.Dot);
            }
            Image image = new Image() { Source = Constants.NoConstraintImage, Width = 10, Height = 10 };
            Canvas.Children.Add(image);
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(this);

            if (moveMode)
            {
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }
    }
}
