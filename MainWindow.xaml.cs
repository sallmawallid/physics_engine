using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SlimDX;

namespace physicsEngine
{
    public partial class MainWindow : Window
    {
        private List<Circle> _circles = new List<Circle>();
        private List<RectangleShape> _rectangles = new List<RectangleShape>();

        public MainWindow()
        {
            InitializeComponent();

            CreateCircle(60f, 60f);
            CreateCircle(150f, 150f);

            CreateRectangle(200f, 200f);
            CreateRectangle(300f, 100f);
        }

        private void CreateCircle(float xPos, float yPos)
        {
            var ellipse = new Ellipse
            {
                Width = 60,
                Height = 60,
                Fill = Brushes.LightBlue
            };
            Circle circle = new Circle(MyCanvas, ellipse);
            circle.Initialize(new Vector3(xPos, yPos, 0));
            _circles.Add(circle);
            MyCanvas.Children.Add(ellipse);

            ellipse.MouseLeftButtonDown += HandleMouseLeftButtonDown;
            ellipse.MouseLeftButtonUp += HandleMouseLeftButtonUp;
            ellipse.MouseMove += HandleMouseMove;
        }

        private void CreateRectangle(float xPos, float yPos)
        {
            var rectangle = new Rectangle
            {
                Width = 80,
                Height = 40,
                Fill = Brushes.LightGreen
            };
            RectangleShape rectShape = new RectangleShape(MyCanvas, rectangle);
            rectShape.Initialize(new Vector3(xPos, yPos, 0));
            _rectangles.Add(rectShape);
            MyCanvas.Children.Add(rectangle);

            rectangle.MouseLeftButtonDown += HandleMouseLeftButtonDown;
            rectangle.MouseLeftButtonUp += HandleMouseLeftButtonUp;
            rectangle.MouseMove += HandleMouseMove;
        }

        public void HandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                var circle = _circles.FirstOrDefault(c => c.GetEllipse() == ellipse);
                circle?.Down(sender, e);
            }
            else if (sender is Rectangle rectangle)
            {
                var rectShape = _rectangles.FirstOrDefault(r => r.GetRectangle() == rectangle);
                rectShape?.Down(sender, e);
            }
        }

        public void HandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                var circle = _circles.FirstOrDefault(c => c.GetEllipse() == ellipse);
                circle?.Up(sender, e);
            }
            else if (sender is Rectangle rectangle)
            {
                var rectShape = _rectangles.FirstOrDefault(r => r.GetRectangle() == rectangle);
                rectShape?.Up(sender, e);
            }
        }

        public void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                var circle = _circles.FirstOrDefault(c => c.GetEllipse() == ellipse);
                circle?.Move(sender, e);
            }
            else if (sender is Rectangle rectangle)
            {
                var rectShape = _rectangles.FirstOrDefault(r => r.GetRectangle() == rectangle);
                rectShape?.Move(sender, e);
            }
        }

        private void selectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (changecolor != null && changecolor.SelectedItem is ComboBoxItem selectedItem)
            {
                string colorName = selectedItem.Content.ToString();
                Brush color = colorName switch
                {
                    "Red" => Brushes.Red,
                    "Cyan" => Brushes.Cyan,
                    "PeachPuff" => Brushes.PeachPuff,
                    "HotPink" => Brushes.HotPink,
                    _ => Brushes.PeachPuff
                };

                foreach (var circle in _circles)
                {
                    circle.ChangeColor(color);
                }

                foreach (var rect in _rectangles)
                {
                    rect.ChangeColor(color);
                }
            }
        }
    }
}
