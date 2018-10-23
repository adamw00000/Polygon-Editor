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
        WriteableBitmap bitmap;

        Vertex firstVertex = null;

        bool drawMode = true;
        bool moveMode = false;
        Vertex movedVertex;

        public ICommand horizontalCommand;
        public ICommand verticalCommand;
        public ICommand angleCommand;
        public ICommand removeCommand;
        public ICommand clearVertexCommand;
        public ICommand clearSideCommand;

        public int BitmapHeight { get; set; }
        public int BitmapWidth { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Canvas.Source = bitmap;
            horizontalCommand = new RelayCommand<Vertex>(v =>
            {
                polygon.AddSideConstraint(v, Vertex.SideConstraint.Horizontal);
                DrawPolygon();
            });
            verticalCommand = new RelayCommand<Vertex>(v =>
            {
                polygon.AddSideConstraint(v, Vertex.SideConstraint.Vertical);
                DrawPolygon();
            });
            angleCommand = new RelayCommand<Vertex>(v =>
            {
                AngleDialog dialog = new AngleDialog() { Angle = v.CalculateAngle() };
                dialog.ShowDialog();
                if (!dialog.IsSet)
                    return;
                polygon.AddAngleConstraint(v, dialog.Angle, Vertex.VertexConstraint.Angle);
                DrawPolygon();
            });
            removeCommand = new RelayCommand<Vertex>(v =>
            {
                polygon.RemoveVertex(v);
                DrawPolygon();
            });
            clearVertexCommand = new RelayCommand<Vertex>(v =>
            {
                polygon.ClearVertexConstraint(v);
                DrawPolygon();
            });
            clearSideCommand = new RelayCommand<Vertex>(v =>
            {
                polygon.ClearSideConstraint(v);
                DrawPolygon();
            });
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (drawMode)
            {
                Vertex clickedVertex = polygon.GetClickedVertex(p);

                if (clickedVertex != null && polygon.Vertices.Count >= 3 && clickedVertex == firstVertex)
                {
                    drawMode = false;
                    DrawPolygon();
                    return;
                }

                Vertex v = new Vertex(p);
                polygon.AddVertex(v);
                DrawPolygon();

                if (firstVertex == null)
                    firstVertex = v;
            }
            else
            {

                Vertex clickedVertex = polygon.GetClickedVertex(p);

                if (clickedVertex != null)
                {
                    moveMode = true;
                    movedVertex = clickedVertex;
                }
                else
                {
                    Vertex clickedSideFirstVertex = polygon.GetClickedSideFirstVertex(p);
                    if (clickedSideFirstVertex != null)
                    {
                        polygon.AddVertexAfter(clickedSideFirstVertex);
                        DrawPolygon();
                    }
                }
            }
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (!drawMode)
            {
                Vertex clickedVertex = polygon.GetClickedVertex(p);
                if (clickedVertex != null)
                {
                    ContextMenu menu = new ContextMenu();
                    menu.Items.Add(new MenuItem()
                    {
                        Header = "Remove vertex",
                        Command = removeCommand,
                        CommandParameter = clickedVertex
                    });
                    if (clickedVertex.Constraint == Vertex.VertexConstraint.None)
                    {
                        menu.Items.Add(new MenuItem()
                        {
                            Header = "Add angle constraint",
                            Command = angleCommand,
                            CommandParameter = clickedVertex
                        });
                    }
                    else
                    {
                        menu.Items.Add(new MenuItem()
                        {
                            Header = "Clear constraint",
                            Command = clearVertexCommand,
                            CommandParameter = clickedVertex
                        });
                    }
                    menu.Margin = new Thickness(p.X, p.Y, 0, 0);
                    menu.IsOpen = true;
                }
                else
                {
                    Vertex clickedSideFirstVertex = polygon.GetClickedSideFirstVertex(p);
                    if (clickedSideFirstVertex != null)
                    {
                        ContextMenu menu = new ContextMenu();
                        if (clickedSideFirstVertex.NextConstraint == Vertex.SideConstraint.None)
                        {
                            menu.Items.Add(new MenuItem()
                            {
                                Header = "Add horizontal constraint",
                                Command = horizontalCommand,
                                CommandParameter = clickedSideFirstVertex
                            });
                            menu.Items.Add(new MenuItem()
                            {
                                Header = "Add vertical constraint",
                                Command = verticalCommand,
                                CommandParameter = clickedSideFirstVertex
                            });
                        }
                        else
                        {
                            menu.Items.Add(new MenuItem()
                            {
                                Header = "Clear constraint",
                                Command = clearSideCommand,
                                CommandParameter = clickedSideFirstVertex
                            });
                        }
                        menu.Margin = new Thickness(p.X, p.Y, 0, 0);
                        menu.IsOpen = true;
                    }
                }
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);
            
            if (moveMode)
            {
                moveMode = false;
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }

        private void DrawPolygon()
        {
            bitmap.Clear();

            foreach (Vertex v in polygon.Vertices.Enumerate())
            {
                DrawDot(new Point(v.X, v.Y), Constants.PointSize);
                if (polygon.Vertices.Count >= 2)
                {
                    if (drawMode && v.Next == firstVertex)
                        return;
                    var line = new MyLine(new Point(v.X, v.Y), new Point(v.Next.X, v.Next.Y));
                    line.Bresenham();
                    DrawLine(line);
                    DrawIcon(v, v.Next);
                }
            }
        }

        private void DrawDot(Point p, int size)
        {
            bitmap.FillEllipse((int)p.X - size / 2, (int)p.Y - size / 2, (int)p.X + size / 2, (int)p.Y + size / 2, Colors.Black);
        }

        private void DrawLine(MyLine line)
        {
            foreach (var point in line.Points)
            {
                bitmap.SetPixel((int)point.X, (int)point.Y, Colors.Black);
            }
        }

        private void DrawIcon(Vertex v1, Vertex v2)
        {
            BitmapSource source = null;
            if (v1.NextConstraint == Vertex.SideConstraint.Horizontal)
                source = Constants.HorizontalImage as BitmapSource;
            else if (v1.NextConstraint == Vertex.SideConstraint.Vertical)
                source = Constants.VerticalImage as BitmapSource;
            else if (v1.Constraint == Vertex.VertexConstraint.Angle || v2.Constraint == Vertex.VertexConstraint.Angle)
                source = Constants.AngleImage as BitmapSource;
            else return;

            int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);
            byte[] data = new byte[stride * source.PixelHeight];

            int X = (int)((v1.X + v2.X - source.Width) / 2);
            int Y = (int)((v1.Y + v2.Y - source.Height) / 2);
            source.CopyPixels(data, stride, 0);
            bitmap.WritePixels(new Int32Rect(X, Y, source.PixelWidth, source.PixelHeight), data, stride, 0);
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (moveMode)
            {
                if (polygon.MoveVertex(movedVertex, p) == false)
                    moveMode = false;
                DrawPolygon();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayout();

            bitmap = new WriteableBitmap((int)10, (int)5, 96, 96, PixelFormats.Bgra32, null);
            Canvas.Source = bitmap;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BitmapWidth = (int)e.NewSize.Width;
            BitmapHeight = (int)e.NewSize.Height;
            polygon.SetBounds(BitmapWidth, BitmapHeight);

            bitmap = bitmap.Resize(BitmapWidth, BitmapHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
            Canvas.Source = bitmap;
        }

        private void NewPolygon_Click(object sender, RoutedEventArgs e)
        {
            polygon = new Polygon();
            polygon.SetBounds(BitmapWidth, BitmapHeight);
            firstVertex = null;
            drawMode = true;
            moveMode = false;
            DrawPolygon();
        }

        private void DeletePolygon_Click(object sender, RoutedEventArgs e)
        {
            NewPolygon_Click(sender, e);
        }
    }
}
