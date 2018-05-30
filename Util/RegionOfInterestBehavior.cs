using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using myzy.Util.Annotations;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using System.Threading;

namespace myzy.Util
{
    public class RegLocation
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return $"X = {X}, Y = {Y}, Width={Width}, Height={Height}";
        }
    }

    public class RegionOfInterestBehavior : Behavior<Image>
    {
        private Canvas _parent;

        #region MARK: Behavior

        public bool MouseWheelChangeSize
        {
            get { return (bool)GetValue(MouseWheelChangeSizeProperty); }
            set { SetValue(MouseWheelChangeSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseWheelChangeSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseWheelChangeSizeProperty =
            DependencyProperty.Register("MouseWheelChangeSize", typeof(bool), typeof(RegionOfInterestBehavior), new PropertyMetadata(false));

        public bool RightMousePanMov
        {
            get { return (bool)GetValue(RightMousePanMovProperty); }
            set { SetValue(RightMousePanMovProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightMousePanMov.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightMousePanMovProperty =
            DependencyProperty.Register("RightMousePanMov", typeof(bool), typeof(RegionOfInterestBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            base.OnAttached();

            _parent = AssociatedObject.Parent as Canvas;

            if (_parent == null)
            {
                throw new NotSupportedException("Image must be located in Canvas.");
            }

            _parent.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            _parent.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            _parent.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;

            _parent.PreviewMouseWheel += _parent_PreviewMouseWheel;

            _parent.PreviewMouseRightButtonDown += _parent_PreviewMouseRightButtonDown;
            _parent.PreviewMouseRightButtonUp += _parent_PreviewMouseRightButtonUp;
            _parent.PreviewMouseMove += _parent_PreviewMouseMove;

            _cursor = AssociatedObject.Cursor;
        }

        private void _parent_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!RightMousePanMov) return;

            var target = AssociatedObject;
            if (!target.IsMouseCaptured)
                return;

            var p = e.MouseDevice.GetPosition(_parent);

            Action<UIElement> el = pElement =>
            {
                if (pElement == null)
                    return;

                var m = pElement.RenderTransform.Value;

                m.OffsetX = _origin.X + p.X - _start.X;
                m.OffsetY = _origin.Y + p.Y - _start.Y;

                pElement.RenderTransform = new MatrixTransform(m);
            };

            el(AssociatedObject);

            RenderRectangles();
        }

        private void RenderRectangles()
        {
            foreach (var pair in _rectList)
            {
                var pElement = pair.Value;

                if (_rectOri.ContainsKey(pair.Key))
                {
                    var ori = _rectOri[pair.Key];

                    RegLocation loca = new RegLocation();
                    if (ConvertToScaleImageClient(ori, ref loca))
                    {
                        var loc = AssociatedObject.TranslatePoint(new Point(loca.X, loca.Y), _parent);
                        Canvas.SetLeft(pElement, loc.X);
                        Canvas.SetTop(pElement, loc.Y);
                    }
                }
            }
        }

        private void _parent_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!RightMousePanMov) return;

            AssociatedObject.ReleaseMouseCapture();
        }

        private Point _origin;
        private Point _start;
        private void _parent_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!RightMousePanMov) return;

            var target = AssociatedObject;
            if (target.IsMouseCaptured)
                return;

            if (_parent.Children.Contains(_rect))
            {
                _parent.Children.Remove(_rect);
                _rect = null;
            }

            target.CaptureMouse();
            _start = e.GetPosition(_parent);
            _origin.X = target.RenderTransform.Value.OffsetX;
            _origin.Y = target.RenderTransform.Value.OffsetY;
        }

        public void ClearScaleInfo()
        {
            Action<UIElement> el = ( target) =>
            {
                var m = target.RenderTransform.Value;
                m.SetIdentity();
                m.OffsetX = 0;
                m.OffsetY = 0;
                target.RenderTransform = new MatrixTransform(m);
            };

            el(_parent);
            el(AssociatedObject);

            _parent.UpdateLayout();

            RenderRectangles();
        }

        private void _parent_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!MouseWheelChangeSize)
                return;

            var target = _parent;
            var p = e.MouseDevice.GetPosition(target);
            var m = target.RenderTransform.Value;

            if (e.Delta > 0)
                m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
            else
                m.ScaleAtPrepend(1/1.1, 1/1.1, p.X, p.Y);

            target.RenderTransform = new MatrixTransform(m);
        }

        protected override void OnDetaching()
        {
            //AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
            //AssociatedObject.MouseMove -= OnMouseMove;
            //AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
            //AssociatedObject.MouseEnter -= OnDragEnter;
            //AssociatedObject.MouseRightButtonUp -= OnMouseRightButtonUp;

            _parent.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            _parent.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;

            _parent.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
            _parent.PreviewMouseWheel -= _parent_PreviewMouseWheel;
            _parent.PreviewMouseRightButtonDown -= _parent_PreviewMouseRightButtonDown;

            _parent.PreviewMouseRightButtonDown -= _parent_PreviewMouseRightButtonDown;
            _parent.PreviewMouseRightButtonUp -= _parent_PreviewMouseRightButtonUp;
            _parent.PreviewMouseMove -= _parent_PreviewMouseMove;

            //AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
            base.OnDetaching();
        }

        #endregion

        private Cursor _cursor;
        private Point? _startlocation;
        private Rectangle _rect = null;
        private readonly Dictionary<string, Rectangle> _rectList = new Dictionary<string, Rectangle>();
        private readonly Dictionary<string, RegLocation> _rectOri = new Dictionary<string, RegLocation>();
        public Color RectColor
        {
            get { return _rectColor; }
            set { _rectColor = value; }
        }

        private void InitRoiRect(Color color, ref Rectangle rect)
        {
            if (rect == null)
            {
                rect = new Rectangle
                {
                    Fill = Brushes.Transparent,
                    StrokeThickness = 3,
                    Stroke = new SolidColorBrush(color),
                    StrokeDashArray = new DoubleCollection() {5, 2},
                    Opacity = 0.8,
                };
                _parent.Children.Add(rect);
            }
        }

        private Color _rectColor = Colors.Red;

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_rect != null)
            {
                _parent.Children.Remove(_rect);
                _rect = null;
                _startlocation = null;
            }
            _startlocation = Mouse.GetPosition(AssociatedObject);

            AssociatedObject.Cursor = Cursors.Cross;
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_startlocation == null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed && AssociatedObject != null)
            {
                var pCur = Mouse.GetPosition(AssociatedObject);
                if (pCur.X <= 0 || pCur.Y <= 0 || pCur.X > AssociatedObject.Width || pCur.Y > AssociatedObject.Height)
                    return;

                if (_startlocation.Value != pCur && _rect == null)
                {
                    InitRoiRect(_rectColor, ref _rect);
                }
                if (_startlocation != null && _rect != null)
                {
                    var offset = pCur - _startlocation.Value;

                    var x = Math.Min(pCur.X, _startlocation.Value.X);
                    var y = Math.Min(pCur.Y, _startlocation.Value.Y);

                    var temp = AssociatedObject.TranslatePoint(new Point(x, y), _parent);

                    var rc = new Rect(temp.X, temp.Y, Math.Abs(offset.X), Math.Abs(offset.Y));
                    SetRectangleOnParent(ref _rect, rc);
                }
            }
        }

        /// <summary>
        /// 在_parent上显示Rectangle， 坐标为Parent上的坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="loc"></param>
        private void SetRectangleOnParent(ref Rectangle rect, Rect loc/*pos on image*/)
        {
            Canvas.SetLeft(rect, loc.X);
            Canvas.SetTop(rect, loc.Y);
            rect.Width = loc.Width;
            rect.Height = loc.Height;
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Released)
                return;

            if (OnRectReleased != null)
            {
                var loc = new RegLocation();
                if (GetRect(ref loc))
                {
                    OnRectReleased?.Invoke(loc);

                    if (_rect != null)
                    {
                        _parent.Children.Remove(_rect);
                        _rect = null;
                        _startlocation = null;
                    }
                }
            }

            _startlocation = null;
            AssociatedObject.Cursor = _cursor;
        }

        public event Action<RegLocation> OnRectReleased;

        public bool ChkRect()
        {
            return _rect != null && AssociatedObject.Source != null;
        }

        public Rect CvtPixToClient(Rect rect, UIElement element)
        {
            var image = AssociatedObject.Source as BitmapImage;
            if (image != null)
            {
                var imagePixWidth = image.PixelWidth;
                var imagePixHeight = image.PixelHeight;

                var x = (int) (rect.X * AssociatedObject.RenderSize.Width / imagePixWidth);
                var y = (int) (rect.Y * AssociatedObject.RenderSize.Height / imagePixHeight);
                var w = (int) (rect.Width * AssociatedObject.RenderSize.Width / imagePixWidth);
                var h = (int) (rect.Height * AssociatedObject.RenderSize.Height / imagePixHeight);

                Rect rc = new Rect() {X = x, Y = y, Width = w, Height = h};

                if (element != AssociatedObject)
                {
                    Point po = new Point(rc.X, rc.Y);
                    po = AssociatedObject.TranslatePoint(po, element);
                    rc.X = po.X;
                    rc.Y = po.Y;
                }
                return rc;
            }
            return Rect.Empty;
        }
        

        public bool GetRect(ref RegLocation location)
        {
            if (!ChkRect())
                return false;

            var image = AssociatedObject.Source as BitmapSource;

            if (image != null)
            {
                var po = new Point(Canvas.GetLeft(_rect), Canvas.GetTop(_rect));
                var po2 = _parent.TranslatePoint(po, AssociatedObject);

                var tmpRect = new Rect
                {
                    X = po2.X,
                    Y = po2.Y,
                    Width = _rect.Width,
                    Height = _rect.Height
                };

                var imagePixWidth = image.PixelWidth;
                var imagePixHeight = image.PixelHeight;

                var roiX = (int)(tmpRect.X * imagePixWidth / AssociatedObject.RenderSize.Width);
                var roiY = (int)(tmpRect.Y * imagePixHeight / AssociatedObject.RenderSize.Height);
                var roiWidth = (int) (tmpRect.Width * imagePixWidth / AssociatedObject.RenderSize.Width);
                var roiHeight = (int) (tmpRect.Height * imagePixHeight / AssociatedObject.RenderSize.Height);

                location.X = roiX;
                location.Y = roiY;
                location.Width = roiWidth;
                location.Height = roiHeight;

                Console.WriteLine($"OImg: -> {location}");
                Console.WriteLine($"OImg: Po2-> {tmpRect}");

                return true;
            }

            return false;
        }

        private bool ConvertToScaleImageClient(RegLocation loca, ref RegLocation localClient)
        {
            bool res = false;
            var image = AssociatedObject.Source as BitmapSource;
            if (image != null)
            {
                var imagePixWidth = image.PixelWidth;
                var imagePixHeight = image.PixelHeight;

                var x = AssociatedObject.RenderSize.Width * loca.X / imagePixWidth;
                var y = AssociatedObject.RenderSize.Height * loca.Y / imagePixHeight;

                var w = AssociatedObject.RenderSize.Width * loca.Width / imagePixWidth;
                var h = AssociatedObject.RenderSize.Height * loca.Height / imagePixHeight;

                localClient.X = (int)x;
                localClient.Y = (int)y;
                localClient.Width = (int) w;
                localClient.Height = (int) h;

                res = true;
            }
            return res;
        }

        /// <summary>
        /// 手动设定ROI区域
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="loca"></param>
        /// <param name="color"></param>
        public void SetRoi(string idx, RegLocation loca, Color color = default(Color))
        {
            var localClient = new RegLocation();

            if (ConvertToScaleImageClient(loca, ref localClient))
            {
                Rectangle rect = null;
                if (_rectList.ContainsKey(idx))
                {
                    rect = _rectList[idx];
                }
                if (_parent.Children.Contains(rect))
                {
                    _parent.Children.Remove(rect);
                    rect = null;
                }

                InitRoiRect(color, ref rect);

                var tmp1 = new Rect(localClient.X, localClient.Y, localClient.Width, localClient.Height);

                Console.WriteLine($"NImg: loc -> {loca}");
                Console.WriteLine($"NImg: -> {tmp1}");

                var p1 = AssociatedObject.TranslatePoint(new Point(tmp1.X, tmp1.Y), _parent);
                var p2 = AssociatedObject.TranslatePoint(new Point(tmp1.X + tmp1.Width, tmp1.Y + tmp1.Height), _parent);
                SetRectangleOnParent(ref rect, new Rect(p1, p2));

                _rectList[idx] = rect;
                _rectOri[idx] = loca;
            }
        }

        public void ReInitRectangle()
        {
            if (_parent.Children.Contains(_rect))
            {
                _parent.Children.Remove(_rect);
                _rect = null;
            }
            _rectList.Values.ToList().ForEach(p =>
            {
                if (_parent.Children.Contains(p))
                {
                    _parent.Children.Remove(p);
                }
            });
            _rectList.Clear();
        }
    }

}
