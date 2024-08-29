using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace physicsEngine
{
    public abstract class ShapeBase
    {
        protected DispatcherTimer timer;
        protected Stopwatch stopwatch;
        protected Canvas canvas;
        protected double acceleration = 9.8;
        protected double velocity;
        protected double left;
        protected double top;
        protected bool isDragging;
        protected Point clickOffset; // Changed from clickPosition
        protected Shape shape;

        public ShapeBase(Canvas canvas, Shape shape)
        {
            this.canvas = canvas;
            this.shape = shape;
            this.left = Canvas.GetLeft(shape);
            this.top = Canvas.GetTop(shape);

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            stopwatch = new Stopwatch();
        }

        public abstract void Timer_Tick(object sender, EventArgs e);
        public abstract void ResetShape();

        public void Start()
        {
            isDragging = false;
            stopwatch.Reset();
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        public virtual void Down(object sender, MouseButtonEventArgs e)
        {
            if (sender is Shape shape)
            {
                Point mousePosition = e.GetPosition(canvas);
                Point shapePosition = new Point(Canvas.GetLeft(shape), Canvas.GetTop(shape));
                clickOffset = new Point(mousePosition.X - shapePosition.X, mousePosition.Y - shapePosition.Y);
                isDragging = true;
                shape.CaptureMouse();
            }
        }

        public virtual void Up(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                if (sender is Shape shape)
                {
                    shape.ReleaseMouseCapture();
                    Start();
                }
            }
        }

        public virtual void Move(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                if (sender is Shape shape)
                {
                    Point mousePosition = e.GetPosition(canvas);
                    left = mousePosition.X - clickOffset.X;
                    top = mousePosition.Y - clickOffset.Y;

                    left = Math.Max(0, Math.Min(left, canvas.ActualWidth - shape.ActualWidth));
                    top = Math.Max(0, Math.Min(top, canvas.ActualHeight - shape.ActualHeight));

                    Canvas.SetLeft(shape, left);
                    Canvas.SetTop(shape, top);
                }
            }
        }
    }
}
