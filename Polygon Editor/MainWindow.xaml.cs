﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
        List<Polygon> polygons = new List<Polygon>();
        //Polygon polygon = new Polygon();
        WriteableBitmap bitmap;

        Vertex firstVertex = null;
        Polygon lastPolygon = null;
        Polygon currentPolygon = null;

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
            lastPolygon = new Polygon();
            polygons.Add(lastPolygon);

            InitializeComponent();
            Canvas.Source = bitmap;
            horizontalCommand = new RelayCommand<Vertex>(v =>
            {
                currentPolygon.AddSideConstraint(v, Vertex.SideConstraint.Horizontal);
                DrawCanvas();
            });
            verticalCommand = new RelayCommand<Vertex>(v =>
            {
                currentPolygon.AddSideConstraint(v, Vertex.SideConstraint.Vertical);
                DrawCanvas();
            });
            angleCommand = new RelayCommand<Vertex>(v =>
            {
                AngleDialog dialog = new AngleDialog() { Angle = v.CalculateAngle() };
                dialog.ShowDialog();
                if (!dialog.IsSet)
                    return;
                currentPolygon.AddAngleConstraint(v, dialog.Angle, Vertex.VertexConstraint.Angle);
                DrawCanvas();
            });
            removeCommand = new RelayCommand<Vertex>(v =>
            {
                currentPolygon.RemoveVertex(v);
                DrawCanvas();
            });
            clearVertexCommand = new RelayCommand<Vertex>(v =>
            {
                currentPolygon.ClearVertexConstraint(v);
                DrawCanvas();
            });
            clearSideCommand = new RelayCommand<Vertex>(v =>
            {
                currentPolygon.ClearSideConstraint(v);
                DrawCanvas();
            });
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (drawMode)
            {
                foreach(var polygon in polygons)
                {
                    Vertex clickedVertex = polygon.GetClickedVertex(p);

                    if (clickedVertex != null && polygon.Vertices.Count >= 3 && clickedVertex == firstVertex)
                    {
                        drawMode = false;
                        DrawCanvas();
                        return;
                    }
                }

                Vertex v = new Vertex(p);
                lastPolygon.AddVertex(v);
                DrawCanvas();

                if (firstVertex == null)
                    firstVertex = v;
            }
            else
            {

                foreach (var polygon in polygons)
                {
                    Vertex clickedVertex = polygon.GetClickedVertex(p);

                    if (clickedVertex != null)
                    {
                        moveMode = true;
                        movedVertex = clickedVertex;
                        currentPolygon = polygon;
                        return;
                    }
                    else
                    {
                        Vertex clickedSideFirstVertex = polygon.GetClickedSideFirstVertex(p);
                        if (clickedSideFirstVertex != null)
                        {
                            polygon.AddVertexAfter(clickedSideFirstVertex);
                            DrawCanvas();
                            return;
                        }
                    }
                }
            }
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (!drawMode)
            {
                foreach (var polygon in polygons)
                {
                    Vertex clickedVertex = polygon.GetClickedVertex(p);
                    if (clickedVertex != null)
                    {
                        currentPolygon = polygon;
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
                            currentPolygon = polygon;
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
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);
            
            if (moveMode)
            {
                moveMode = false;
                currentPolygon.MoveVertex(movedVertex, p);
                DrawCanvas();
            }
        }

        private void DrawCanvas()
        {
            bitmap.Clear();
            foreach (var polygon in polygons)
                foreach (Vertex v in polygon.Vertices.Enumerate())
                {
                    DrawDot(new Point(v.X, v.Y), Constants.PointSize);
                    if (polygon.Vertices.Count >= 2)
                    {
                        if (drawMode && v.Next == firstVertex)
                            return;
                        bitmap.Bresenham(v.X, v.Y, v.Next.X, v.Next.Y);
                    
                        DrawIcon(v, v.Next);
                    }
                }
        }

        private void DrawDot(Point p, int size)
        {
            bitmap.FillEllipse((int)p.X - size / 2, (int)p.Y - size / 2, (int)p.X + size / 2, (int)p.Y + size / 2, Colors.Black);
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
            int X;
            int Y;
            if (v1.Constraint == Vertex.VertexConstraint.Angle)
            {
                X = (int)((2 * v1.X + v2.X) / 3 - source.Width / 2);
                Y = (int)((2 * v1.Y + v2.Y) / 3 - source.Height / 2);
            }
            else if (v2.Constraint == Vertex.VertexConstraint.Angle)
            {
                X = (int)((v1.X + 2 * v2.X) / 3 - source.Width / 2);
                Y = (int)((v1.Y + 2 * v2.Y) / 3 - source.Height / 2);
            }
            else
            {
                X = (int)((v1.X + v2.X - source.Width) / 2);
                Y = (int)((v1.Y + v2.Y - source.Height) / 2);
            }
            
            source.CopyPixels(data, stride, 0);
            
            if (X < 0)
            {
                X = 0;
            }
            if (Y < 0)
            {
                Y = 0;
            }
            if (X > BitmapWidth - source.PixelWidth)
            {
                X = BitmapWidth - source.PixelWidth;
            }
            if (Y > BitmapHeight - source.PixelHeight)
            {
                Y = BitmapHeight - source.PixelHeight;
            }

            bitmap.WritePixels(new Int32Rect(X, Y, source.PixelWidth, source.PixelHeight), data, stride, 0);
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (moveMode)
            {
                if (currentPolygon.MoveVertex(movedVertex, p) == false)
                    moveMode = false;
                DrawCanvas();
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

            foreach(var polygon in polygons)
                polygon.SetBounds(BitmapWidth, BitmapHeight);

            bitmap = bitmap.Resize(BitmapWidth, BitmapHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
            Canvas.Source = bitmap;
        }

        private void NewPolygon_Click(object sender, RoutedEventArgs e)
        {
            lastPolygon = new Polygon();
            lastPolygon.SetBounds(BitmapWidth, BitmapHeight);
            polygons.Add(lastPolygon);
            firstVertex = null;
            drawMode = true;
            moveMode = false;
            DrawCanvas();
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            polygons.Clear();
            lastPolygon = new Polygon();
            lastPolygon.SetBounds(BitmapWidth, BitmapHeight);
            polygons.Add(lastPolygon);
            firstVertex = null;
            drawMode = true;
            moveMode = false;
            DrawCanvas();
        }

        private void SaveCanvas_Click(object sender, RoutedEventArgs e)
        {
            if (drawMode)
            {
                MessageBox.Show("Cannot save canvas in draw mode!");
                return;
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(List<Polygon>));
            using (FileStream stream = new FileStream("canvas.xml", FileMode.Create))
                serializer.WriteObject(stream, polygons);

            MessageBox.Show("Saving successful!");
        }

        private void LoadCanvas_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("canvas.xml"))
            {
                MessageBox.Show("canvas.xml doesn't exist!");
                return;
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(List<Polygon>));
            using (FileStream stream = new FileStream("canvas.xml", FileMode.Open))
                polygons = serializer.ReadObject(stream) as List<Polygon>;

            drawMode = false;
            foreach (var polygon in polygons)
                polygon.SetBounds(BitmapWidth, BitmapHeight);
            DrawCanvas();
            MessageBox.Show("Loading successful!");
        }
    }
}
