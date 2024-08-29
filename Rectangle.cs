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
    public class RectangleShape : ShapeBase
    {
        private Rectangle _rectangle;
        private float _left;
        private float _top;
        private Vector3 _velocity = new Vector3(0, 0, 0);
        private Vector3 _acceleration = new Vector3(0, 30.0f, 0); 
        private bool _isDragging;
        private float bouncing = 0.9f; 

        public RectangleShape(Canvas canvas, Rectangle rectangle) : base(canvas, rectangle)
        {
            _rectangle = rectangle;
        }

        public void Initialize(Vector3 position)
        {
            _left = position.X;
            _top = position.Y;
            Canvas.SetLeft(_rectangle, _left);
            Canvas.SetTop(_rectangle, _top);
        }

        public override void Timer_Tick(object sender, EventArgs e)
        {
            if (stopwatch.IsRunning && !_isDragging)
            {
                double elapsedTime = stopwatch.Elapsed.TotalSeconds;

                
                _velocity += _acceleration * (float)elapsedTime;
                _left += _velocity.X * (float)elapsedTime;
                _top += _velocity.Y * (float)elapsedTime;

                
                HandleBoundaryCollisions();
                CheckCollision();

                Canvas.SetTop(_rectangle, _top);
                Canvas.SetLeft(_rectangle, _left);

                stopwatch.Restart();
            }
        }

        private void HandleBoundaryCollisions()
        {
            if (_top + _rectangle.ActualHeight > canvas.ActualHeight)
            {
                _top = (float)(canvas.ActualHeight - _rectangle.ActualHeight);
                _velocity.Y = -_velocity.Y * bouncing                                                                                                  ;
            }
            else if (_top < 0)
            {
                _top = 0;
                _velocity.Y = -_velocity.Y * bouncing;
            }

            if (_left + _rectangle.ActualWidth > canvas.ActualWidth)
            {
                _left = (float)(canvas.ActualWidth - _rectangle.ActualWidth);
                _velocity.X = -_velocity.X * bouncing;
            }
            else if (_left < 0)
            {
                _left = 0;
                _velocity.X = -_velocity.X * bouncing;
            }
        }

        private void CheckCollision()
        {
            foreach (UIElement element in canvas.Children)
            {
                if (element is Rectangle otherRectangle && otherRectangle != _rectangle)
                {
                    HandleCollision(otherRectangle);
                }
            }
        }

        private void HandleCollision(Rectangle otherRectangle)
        {
            double otherLeft = Canvas.GetLeft(otherRectangle);
            double otherTop = Canvas.GetTop(otherRectangle);
            double otherWidth = otherRectangle.ActualWidth;
            double otherHeight = otherRectangle.ActualHeight;

            double rectWidth = _rectangle.ActualWidth;
            double rectHeight = _rectangle.ActualHeight;

            // Check for overlap
            if (_left < otherLeft + otherWidth &&
                _left + rectWidth > otherLeft &&
                _top < otherTop + otherHeight &&
                _top + rectHeight > otherTop)
            {
                // Resolve overlap
                // Simple overlap correction
                double overlapX = Math.Min(_left + rectWidth - otherLeft, otherLeft + otherWidth - _left);
                double overlapY = Math.Min(_top + rectHeight - otherTop, otherTop + otherHeight - _top);

                if (overlapX < overlapY)
                {
                    _left -= (float)overlapX * Math.Sign(_velocity.X);
                    _velocity.X = -_velocity.X * bouncing;
                }
                else
                {
                    _top -= (float)overlapY * Math.Sign(_velocity.Y);
                    _velocity.Y = -_velocity.Y * bouncing;
                }

                Canvas.SetLeft(_rectangle, _left);
                Canvas.SetTop(_rectangle, _top);
            }
        }

        public override void ResetShape()
        {
            _left = (float)Canvas.GetLeft(_rectangle);
            _top = (float)Canvas.GetTop(_rectangle);
            stopwatch.Restart();
            timer.Stop();
            timer.Start();
        }

        public void ChangeColor(Brush color)
        {
            _rectangle.Fill = color;
        }

        internal Rectangle GetRectangle()
        {
            return _rectangle;
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

                _left = (float)Canvas.GetLeft(_rectangle);
                _top = (float)Canvas.GetTop(_rectangle);
            }
        }
    }
}
