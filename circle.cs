using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using SlimDX;

namespace physicsEngine
{
    public class Circle : ShapeBase
    {
        private Ellipse _circle;
        private float _left;
        private float _top;
        private Vector3 _velocity = new Vector3(0, 0, 0);
        private Vector3 _acceleration = new Vector3(0, 30.0f, 0); 
        private bool _isDragging;

        public Circle(Canvas canvas, Ellipse circle) : base(canvas, circle)
        {
            _circle = circle;
        }

        public void Initialize(Vector3 position)
        {
            _left = position.X;
            _top = position.Y;
            Canvas.SetLeft(_circle, _left);
            Canvas.SetTop(_circle, _top);
        }

        public override void Timer_Tick(object sender, EventArgs e)
        {
            if (stopwatch.IsRunning && !_isDragging)
            {
                double elapsedTime = stopwatch.Elapsed.TotalSeconds;

                _velocity += _acceleration * (float)elapsedTime;
                _left += _velocity.X * (float)elapsedTime;
                _top += _velocity.Y * (float)elapsedTime;

                if (_top + _circle.ActualHeight > canvas.ActualHeight)
                {
                    _top = (float)(canvas.ActualHeight - _circle.ActualHeight);
                    _velocity.Y = -_velocity.Y * 0.9f; 
                }
                else if (_top < 0)
                {
                    _top = 0;
                    _velocity.Y = -_velocity.Y * 0.9f; 
                }

                if (_left + _circle.ActualWidth > canvas.ActualWidth)
                {
                    _left = (float)(canvas.ActualWidth - _circle.ActualWidth);
                    _velocity.X = -_velocity.X * 0.9f; 
                }
                else if (_left < 0)
                {
                    _left = 0;
                    _velocity.X = -_velocity.X * 0.9f; 
                }

                Canvas.SetTop(_circle, _top);
                Canvas.SetLeft(_circle, _left);

                CheckCollision();
                stopwatch.Restart();
            }
        }

        private void CheckCollision()
        {
            foreach (UIElement element in canvas.Children)
            {
                if (element is Ellipse otherCircle && otherCircle != _circle)
                {
                    HandleCollision(otherCircle);
                }
            }
        }

        private void HandleCollision(Ellipse otherCircle)
        {
            double otherLeft = Canvas.GetLeft(otherCircle);
            double otherTop = Canvas.GetTop(otherCircle);
            double otherRadius = otherCircle.ActualWidth / 2;

            double circleRadius = _circle.ActualWidth / 2;

            double deltaX = (_left + circleRadius) - (otherLeft + otherRadius);
            double deltaY = (_top + circleRadius) - (otherTop + otherRadius);

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance < circleRadius + otherRadius)
            {
                double overlap = circleRadius + otherRadius - distance;
                double adjustX = deltaX / distance * overlap / 2;
                double adjustY = deltaY / distance * overlap / 2;

                _left += (float)adjustX;
                _top += (float)adjustY;

                Canvas.SetLeft(_circle, _left);
                Canvas.SetTop(_circle, _top);

                double normalX = deltaX / distance;
                double normalY = deltaY / distance;

                Vector3 velocity = _velocity;
                double dotProduct = (velocity.X * (float)normalX + velocity.Y * (float)normalY);
                Vector3 reflection = velocity - 2 * (float)dotProduct * new Vector3((float)normalX, (float)normalY, 0);

                _velocity = reflection * 0.9f;
            }
        }

        public override void ResetShape()
        {
            _left = (float)Canvas.GetLeft(_circle);
            _top = (float)Canvas.GetTop(_circle);
            stopwatch.Restart();
            timer.Stop();
            timer.Start();
        }

        public void ChangeColor(Brush color)
        {
            _circle.Fill = color;
        }

        internal Ellipse GetEllipse()
        {
            return _circle;
        }

        public override void Down(object sender, MouseButtonEventArgs e)
        {
            base.Down(sender, e);
            _isDragging = true; 
        }

        public override void Up(object sender, MouseButtonEventArgs e)
        {
            base.Up(sender, e);
            _isDragging = false; 
        }

        public override void Move(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                base.Move(sender, e);
                
                _left = (float)Canvas.GetLeft(_circle);
                _top = (float)Canvas.GetTop(_circle);
            }
        }
    }
}
