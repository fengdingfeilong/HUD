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

namespace HUD
{
    public partial class HUDControl : UserControl
    {
        public HUDControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 姿态改变
        /// </summary>
        private static void GestureChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HUDControl)d).InvalidateVisual();
        }

        /// <summary>
        /// 航向角
        /// </summary>
        public double YawAngle
        {
            get { return (double)GetValue(YawAngleProperty); }
            set { SetValue(YawAngleProperty, value); }
        }
        public static readonly DependencyProperty YawAngleProperty =
            DependencyProperty.Register("YawAngle", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));

        ///// <summary>
        ///// 翻转角
        ///// </summary>
        public double RollAngle
        {
            get { return (double)GetValue(RollAngleProperty); }
            set { SetValue(RollAngleProperty, value); }
        }
        public static readonly DependencyProperty RollAngleProperty =
            DependencyProperty.Register("RollAngle", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 翻转角范围
        /// </summary>
        public double MaxRollAngle
        {
            get { return (double)GetValue(MaxRollAngleProperty); }
            set { SetValue(MaxRollAngleProperty, value); }
        }
        public static readonly DependencyProperty MaxRollAngleProperty =
            DependencyProperty.Register("MaxRollAngle", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)180, GestureChangedCallback));

        ///// <summary>
        ///// 俯仰角
        ///// </summary>
        public double PitchAngle
        {
            get { return (double)GetValue(PitchAngleProperty); }
            set { SetValue(PitchAngleProperty, value); }
        }
        public static readonly DependencyProperty PitchAngleProperty =
            DependencyProperty.Register("PitchAngle", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback), PitchAngleValidateValueCallback);
        private static bool PitchAngleValidateValueCallback(object value)
        {
            bool v = (double)value >= -90 && (double)value <= 90;
            if (!v) throw new Exception("俯仰角必须在-90~90度之间");
            return v;
        }

        private int tickcount = 9;
        private void DrawStaffTicks(Canvas yawstaffCanvas, bool isrightcanvas)
        {
            yawstaffCanvas.Children.Clear();
            double wl = yawstaffCanvas.ActualWidth;
            Line left_l1 = new Line();
            left_l1.X1 = 0;
            left_l1.Y1 = 30;
            left_l1.X2 = wl;
            left_l1.Y2 = 30;
            left_l1.Stroke = Brushes.White;
            left_l1.StrokeThickness = 1;
            yawstaffCanvas.Children.Add(left_l1);
            for (int d = 1; d < 90 / tickcount - 1; d++)
            {
                Line left_tl = new Line();
                left_tl.X1 = d * wl / tickcount;
                left_tl.Y1 = 20;
                left_tl.X2 = left_tl.X1;
                left_tl.Y2 = 30;
                left_tl.Stroke = Brushes.White;
                left_tl.StrokeThickness = 1;
                yawstaffCanvas.Children.Add(left_tl);
                if ((isrightcanvas && d % 2 == 1) || (!isrightcanvas && d % 2 == 0)) continue;//间隔显示
                var ticktext = new BorderTextLabel();
                ticktext.FontWeight = FontWeights.ExtraBold;
                ticktext.Stroke = Brushes.Gray;
                ticktext.FontSize = 14;
                if (isrightcanvas)
                    ticktext.Text = (d * 90 / tickcount).ToString();
                else
                    ticktext.Text = (90 - d * 90 / tickcount).ToString();
                ticktext.Foreground = Brushes.White;
                Canvas.SetTop(ticktext, 2);
                Canvas.SetLeft(ticktext, left_tl.X1 - 10);
                yawstaffCanvas.Children.Add(ticktext);
            }
        }

        private void SetYaw(double yaw)
        {
            if (Grid_YawStaff.ActualWidth == 0) return;
            yaw = yaw % 360;
            if (yaw > 180) yaw = yaw - 360;
            if (yaw < -180) yaw = yaw + 360;
            double left = Grid_YawStaff.ActualWidth / 2 - 15;
            if (yaw > 90 || yaw < -90)
            {
                TextBlock_YawStaff_Left.Text = "E";
                TextBlock_YawStaff_Middle.Text = "S";
                TextBlock_YawStaff_Right.Text = "W";
                if (yaw != 180 && yaw != -180)
                {
                    if (yaw > 0) left = Grid_YawStaff.ActualWidth / 2 - 30 - (180 - yaw) / 90 * Canvas_YawStaff_Right.ActualWidth;
                    else if (yaw < 0) left = Grid_YawStaff.ActualWidth / 2 + (180 + yaw) / 90 * Canvas_YawStaff_Right.ActualWidth;
                }
            }
            else
            {
                TextBlock_YawStaff_Left.Text = "W";
                TextBlock_YawStaff_Middle.Text = "N";
                TextBlock_YawStaff_Right.Text = "E";
                if (yaw == 90) left = Grid_YawStaff.ActualWidth - 30;
                else if (yaw == -90) left = 0;
                else if (yaw > 0) left = Grid_YawStaff.ActualWidth / 2 + yaw / 90 * Canvas_YawStaff_Right.ActualWidth;
                else if (yaw < 0) left = Grid_YawStaff.ActualWidth / 2 - 30 + yaw / 90 * Canvas_YawStaff_Right.ActualWidth;
            }
            Canvas_YawStaff_Value.Margin = new Thickness(left, 0, 0, 0);
            Text_YawStaff_Value.Text = Math.Abs(yaw % 90).ToString("0.##");
            Text_YawStaff_Value.ToolTip = "原始值：" + YawAngle.ToString("0.###");
        }

        protected virtual void RedrawYaw()
        {
            DrawStaffTicks(Canvas_YawStaff_Left, false);
            DrawStaffTicks(Canvas_YawStaff_Right, true);
            SetYaw(YawAngle);
        }

        private void DrawRollTick(double cycleR, double angle)
        {
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 0;
            line.Y1 = 6;
            line.Y2 = 0;
            line.Stroke = Brushes.White;
            line.StrokeThickness = 2;
            Canvas.SetTop(line, -cycleR);
            line.RenderTransform = new RotateTransform(angle, 0, cycleR);
            Canvas_ViewPortMiddle.Children.Add(line);
            var textblock = new BorderTextLabel();
            textblock.Width = 20;
            textblock.Stroke = Brushes.DimGray;
            textblock.HorizontalContentAlignment = HorizontalAlignment.Center;
            textblock.Text = Math.Abs(angle).ToString("##0");
            textblock.Foreground = Brushes.White;
            textblock.FontSize = 14;
            textblock.FontWeight = FontWeights.Bold;
            Canvas.SetTop(textblock, -cycleR - 20);
            Canvas.SetLeft(textblock, -textblock.Width / 2);
            textblock.RenderTransform = new RotateTransform(angle, textblock.Width / 2, cycleR + 20);
            Canvas_ViewPortMiddle.Children.Add(textblock);
        }
        private void DrawPitchTick(double pitch, double offset)
        {
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 40;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stroke = Brushes.White;
            line.StrokeThickness = 2;
            Canvas.SetLeft(line, -20);
            Canvas.SetTop(line, offset);
            Canvas_ViewPortMiddle.Children.Add(line);
            var textblock = new BorderTextLabel();
            textblock.Width = 22;
            textblock.Stroke = Brushes.DimGray;
            textblock.HorizontalContentAlignment = HorizontalAlignment.Center;
            textblock.Text = pitch.ToString("##0");
            textblock.Foreground = Brushes.White;
            textblock.FontSize = 16;
            textblock.FontWeight = FontWeights.Bold;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetLeft(textblock, -textblock.Width - 26);
            Canvas_ViewPortMiddle.Children.Add(textblock);
        }
        private void DrawShortPitchTick(double pitch, double offset)
        {
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 20;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stroke = Brushes.White;
            line.StrokeThickness = 1;
            Canvas.SetLeft(line, -10);
            Canvas.SetTop(line, offset);
            Canvas_ViewPortMiddle.Children.Add(line);
        }
        private void DrawPitchValue(double pitch, double offset)
        {
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 26;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;
            Canvas.SetTop(line, offset);
            Canvas_ViewPortMiddle.Children.Add(line);
            TextBlock textblock = new TextBlock();
            textblock.Width = 40;
            textblock.TextAlignment = TextAlignment.Center;
            textblock.Text = pitch.ToString("##0.#");
            textblock.Foreground = Brushes.White;
            textblock.Background = Brushes.Red;
            textblock.FontSize = 12;
            textblock.FontWeight = FontWeights.Bold;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetLeft(textblock, 26);
            Canvas_ViewPortMiddle.Children.Add(textblock);
        }
        private void DrawRollPitchCycle()
        {
            Canvas_ViewPortMiddle.Children.Clear();
            bool isLargeArc = MaxRollAngle > 90;
            //double cycleR = Grid_Virwport.ActualWidth / 4;
            double cycleR = 100;
            Point startPoint = new Point(-cycleR * Math.Sin(MaxRollAngle * Math.PI / 180), -cycleR * Math.Cos(MaxRollAngle * Math.PI / 180));
            Point endPoint = new Point(cycleR * Math.Sin(MaxRollAngle * Math.PI / 180), -cycleR * Math.Cos(MaxRollAngle * Math.PI / 180));
            if (MaxRollAngle == 180)
            {
                startPoint = new Point(-0.1, cycleR);
                endPoint = new Point(0.1, cycleR);
            }
            ArcSegment arcpath = new ArcSegment(endPoint, new Size(cycleR, cycleR), 0, isLargeArc, SweepDirection.Clockwise, true);
            PathGeometry geometry = new PathGeometry(new PathFigure[] { new PathFigure(startPoint, new PathSegment[] { arcpath }, false) });
            Path cyclepath = new Path();
            cyclepath.Data = geometry;
            cyclepath.Stroke = Brushes.White;
            cyclepath.StrokeThickness = 2;
            int tickangle = 10;
            DrawRollTick(cycleR, 0);
            for (int angle = tickangle; angle <= MaxRollAngle; angle += tickangle)
            {
                if (angle == 180) break;
                DrawRollTick(cycleR, angle);
                DrawRollTick(cycleR, -angle);
            }
            if (MaxRollAngle == 180)
                DrawRollTick(cycleR, MaxRollAngle);
            Canvas_ViewPortMiddle.Children.Add(cyclepath);

            #region 俯仰角
            int pitch = (((int)PitchAngle) / 10) * 10;
            int pitchcount = 3;
            int pitchspace = 5;
            DrawPitchTick(pitch, 0);
            for (int i = 1; i < pitchcount; i++)
            {
                DrawPitchTick(pitch + i * pitchspace, -cycleR * i / pitchcount);
                for (int j = 1; j < pitchspace; j++)
                    DrawShortPitchTick(pitch + (i - 1) * pitchspace + j, -cycleR * ((i - 1) + (float)j / pitchspace) / pitchcount);
                DrawPitchTick(pitch - i * pitchspace, cycleR * i / pitchcount);
                for (int j = 1; j < pitchspace; j++)
                    DrawShortPitchTick(pitch + (i - 1) * pitchspace + j, cycleR * ((i - 1) + (float)j / pitchspace) / pitchcount);
            }
            DrawPitchValue(PitchAngle, -(PitchAngle - pitch) * cycleR / (pitchspace * pitchcount));
            #endregion

            Canvas_ViewPortMiddle.RenderTransform = new RotateTransform(-RollAngle);

            Canvas.SetTop(Canvas_RollCursor, -cycleR - Canvas_RollCursor.Height);
            Canvas.SetLeft(Canvas_RollCursor, -Canvas_RollCursor.Width / 2);
            Text_RollStaff_Value.Text = RollAngle.ToString("0.##");
        }

        protected virtual void RedrawRoll()
        {
            if (Grid_Virwport.ActualHeight == 0 || Grid_Virwport.ActualWidth == 0) return;
            var bkbrush = (LinearGradientBrush)Grid_Virwport.Background;
            double roll = (RollAngle % 360) * Math.PI / 180;
            if (roll > Math.PI) roll = Math.PI * 2 - roll;
            if (roll < -Math.PI) roll = Math.PI * 2 + roll;
            double oppositeangle = Math.Atan(Grid_Virwport.ActualWidth / Grid_Virwport.ActualHeight);
            double startx = 0, starty = 0;
            if (roll >= -oppositeangle && roll <= oppositeangle)
            {
                startx = 0.5 * Grid_Virwport.ActualWidth - 0.5 * Grid_Virwport.ActualHeight * Math.Tan(roll);
                starty = 0;
            }
            else if (roll >= Math.PI - oppositeangle || roll <= -Math.PI + oppositeangle)
            {
                if (roll > 0)
                    startx = 0.5 * Grid_Virwport.ActualWidth - 0.5 * Grid_Virwport.ActualHeight * Math.Tan(Math.PI - roll);
                else
                    startx = 0.5 * Grid_Virwport.ActualWidth + 0.5 * Grid_Virwport.ActualHeight * Math.Tan(Math.PI + roll);
                starty = Grid_Virwport.ActualHeight;
            }
            else if (roll > oppositeangle && roll < Math.PI - oppositeangle)
            {
                startx = 0;
                starty = 0.5 * Grid_Virwport.ActualHeight - 0.5 * Grid_Virwport.ActualWidth / Math.Tan(roll);
            }
            else if (roll > -Math.PI + oppositeangle && roll < -oppositeangle)
            {
                startx = Grid_Virwport.ActualWidth;
                starty = 0.5 * Grid_Virwport.ActualHeight + 0.5 * Grid_Virwport.ActualWidth / Math.Tan(roll);
            }
            bkbrush.StartPoint = new Point(startx, starty);
            bkbrush.EndPoint = new Point(Grid_Virwport.ActualWidth - startx, Grid_Virwport.ActualHeight - starty);

            DrawRollPitchCycle();
        }

        protected virtual void RedrawPitch()
        {
            var bkbrush = (LinearGradientBrush)Grid_Virwport.Background;
            double offset = 0.5;
            double pitch = PitchAngle * Math.PI / 180;
            if (pitch > Math.PI / 3) pitch = Math.PI / 3;//设置俯仰视觉为120度
            if (pitch < -Math.PI / 3) pitch = -Math.PI / 3;
            offset = 0.5 * (1 + Math.Tan(pitch) / Math.Tan(Math.PI / 3));
            bkbrush.GradientStops[1].Offset = offset;
            bkbrush.GradientStops[2].Offset = offset;

            DrawRollPitchCycle();
        }

        #region 其他杂项
        /// <summary>
        /// 海拔（高度）
        /// </summary>
        public double FlyHeight
        {
            get { return (double)GetValue(FlyHeightProperty); }
            set { SetValue(FlyHeightProperty, value); }
        }
        public static readonly DependencyProperty FlyHeightProperty =
            DependencyProperty.Register("FlyHeight", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 空速
        /// </summary>
        public double AirSpeed
        {
            get { return (double)GetValue(AirSpeedProperty); }
            set { SetValue(AirSpeedProperty, value); }
        }
        public static readonly DependencyProperty AirSpeedProperty =
            DependencyProperty.Register("AirSpeed", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 地速
        /// </summary>
        public double GroundSpeed
        {
            get { return (double)GetValue(GroundSpeedProperty); }
            set { SetValue(GroundSpeedProperty, value); }
        }
        public static readonly DependencyProperty GroundSpeedProperty =
            DependencyProperty.Register("GroundSpeed", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 电压
        /// </summary>
        public double Voltage
        {
            get { return (double)GetValue(VoltageProperty); }
            set { SetValue(VoltageProperty, value); }
        }
        public static readonly DependencyProperty VoltageProperty =
            DependencyProperty.Register("Voltage", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 电流
        /// </summary>
        public double Galvanic
        {
            get { return (double)GetValue(GalvanicProperty); }
            set { SetValue(GalvanicProperty, value); }
        }
        public static readonly DependencyProperty GalvanicProperty =
            DependencyProperty.Register("Galvanic", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 剩余电量百分比
        /// </summary>
        public double BatteryPercent
        {
            get { return (double)GetValue(BatteryPercentProperty); }
            set { SetValue(BatteryPercentProperty, value); }
        }
        public static readonly DependencyProperty BatteryPercentProperty =
            DependencyProperty.Register("BatteryPercent", typeof(double), typeof(HUDControl), new FrameworkPropertyMetadata((double)1, GestureChangedCallback));
        /// <summary>
        /// 飞行时间
        /// </summary>
        public int FlyTime
        {
            get { return (int)GetValue(FlyTimeProperty); }
            set { SetValue(FlyTimeProperty, value); }
        }
        public static readonly DependencyProperty FlyTimeProperty =
            DependencyProperty.Register("FlyTime", typeof(int), typeof(HUDControl), new FrameworkPropertyMetadata((int)0, GestureChangedCallback));
        /// <summary>
        /// 是否有EKF
        /// </summary>
        public bool HasEKF
        {
            get { return (bool)GetValue(HasEKFProperty); }
            set { SetValue(HasEKFProperty, value); }
        }
        public static readonly DependencyProperty HasEKFProperty =
            DependencyProperty.Register("HasEKF", typeof(bool), typeof(HUDControl), new FrameworkPropertyMetadata((bool)true, GestureChangedCallback));
        /// <summary>
        /// 是否有Vibe
        /// </summary>
        public bool HasVibe
        {
            get { return (bool)GetValue(HasVibeProperty); }
            set { SetValue(HasVibeProperty, value); }
        }
        public static readonly DependencyProperty HasVibeProperty =
            DependencyProperty.Register("HasVibe", typeof(bool), typeof(HUDControl), new FrameworkPropertyMetadata((bool)true, GestureChangedCallback));
        /// <summary>
        /// 是否有GPS
        /// </summary>
        public bool HasGPS
        {
            get { return (bool)GetValue(HasGPSProperty); }
            set { SetValue(HasGPSProperty, value); }
        }
        public static readonly DependencyProperty HasGPSProperty =
            DependencyProperty.Register("HasGPS", typeof(bool), typeof(HUDControl), new FrameworkPropertyMetadata((bool)false, GestureChangedCallback));


        //绘制右边海拔（高度）矩形
        private void RedrawHeight()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 50;
            rectangle.Height = 150;
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = Brushes.White;
            Canvas.SetTop(rectangle, -rectangle.Height / 2);
            Canvas.SetRight(rectangle, 0);
            Canvas_ViewPortRight.Children.Add(rectangle);
            double from = FlyHeight - FlyHeight % 5 + 15;
            double space = (rectangle.Height - 20) / 5;
            for (int i = 0; i < 6; i++)
            {
                Line li = new Line();
                li.X1 = -rectangle.Width;
                li.Y1 = -rectangle.Height / 2 + 10 + space * i;
                li.X2 = li.X1 + 10;
                li.Y2 = li.Y1;
                li.StrokeThickness = 2;
                li.Stroke = Brushes.White;
                Canvas_ViewPortRight.Children.Add(li);
                BorderTextLabel texti = new BorderTextLabel();
                texti.Width = 22;
                texti.Stroke = Brushes.DimGray;
                texti.HorizontalContentAlignment = HorizontalAlignment.Left;
                texti.Text = (from - i * 5).ToString("##0");
                texti.Foreground = Brushes.White;
                texti.FontSize = 16;
                texti.FontWeight = FontWeights.Bold;
                Canvas.SetLeft(texti, li.X2 + 4);
                Canvas.SetTop(texti, li.Y1 - 10);
                Canvas_ViewPortRight.Children.Add(texti);
            }

            Line l1 = new Line();
            l1.X1 = -rectangle.Width + 1;
            l1.Y1 = -rectangle.Height / 2 + 1;
            l1.X2 = l1.X1 - 16;
            l1.Y2 = l1.Y1 + 16;
            l1.StrokeThickness = 2;
            l1.Stroke = Brushes.White;
            Canvas_ViewPortRight.Children.Add(l1);
            Line l2 = new Line();
            l2.X1 = l1.X2;
            l2.Y1 = l1.Y2 - 1;
            l2.X2 = l2.X1;
            l2.Y2 = l2.Y1 + rectangle.Height - 32;
            l2.StrokeThickness = 2;
            l2.Stroke = Brushes.White;
            Canvas_ViewPortRight.Children.Add(l2);
            Line l3 = new Line();
            l3.X1 = l2.X2;
            l3.Y1 = l2.Y2 - 1;
            l3.X2 = l1.X1;
            l3.Y2 = l1.Y1 + rectangle.Height - 2;
            l3.StrokeThickness = 2;
            l3.Stroke = Brushes.White;
            Canvas_ViewPortRight.Children.Add(l3);
            space = (rectangle.Height - 36) / 9;
            for (int i = 0; i < 10; i++)
            {
                Line li = new Line();
                li.X1 = l1.X2;
                li.X2 = li.X1 + 8;
                li.Y1 = l1.Y2 + 2 + space * i;
                li.Y2 = li.Y1;
                li.StrokeThickness = 2;
                li.Stroke = Brushes.White;
                Canvas_ViewPortRight.Children.Add(li);
            }

            TextBlock textblock = new TextBlock();
            textblock.Width = 30;
            textblock.Padding = new Thickness(0, 0, 2, 0);
            textblock.TextAlignment = TextAlignment.Right;
            textblock.Text = FlyHeight.ToString("##0.#");
            textblock.Foreground = Brushes.White;
            textblock.Background = Brushes.Red;
            textblock.FontSize = 12;
            textblock.FontWeight = FontWeights.Bold;
            double offset = (from - FlyHeight) / 25 * (rectangle.Height - 20) - rectangle.Height / 2 + 10;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetRight(textblock, 0);
            Canvas_ViewPortRight.Children.Add(textblock);
        }

        //绘制左边速度（空速 ）矩形
        private void RedrawSpeed()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 50;
            rectangle.Height = 150;
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = Brushes.White;
            Canvas.SetTop(rectangle, -rectangle.Height / 2);
            Canvas.SetLeft(rectangle, 0);
            Canvas_ViewPortLeft.Children.Add(rectangle);
            double from = AirSpeed - AirSpeed % 5 + 15;
            if (from < 25) from = 25;
            double space = (rectangle.Height - 20) / 5;
            for (int i = 0; i < 6; i++)
            {
                Line li = new Line();
                li.X1 = rectangle.Width;
                li.Y1 = -rectangle.Height / 2 + 10 + space * i;
                li.X2 = li.X1 - 10;
                li.Y2 = li.Y1;
                li.StrokeThickness = 2;
                li.Stroke = Brushes.White;
                Canvas_ViewPortLeft.Children.Add(li);
                BorderTextLabel texti = new BorderTextLabel();
                texti.Width = 22;
                texti.Stroke = Brushes.DimGray;
                texti.HorizontalContentAlignment = HorizontalAlignment.Right;
                texti.Text = (from - i * 5).ToString("##0");
                texti.Foreground = Brushes.White;
                texti.FontSize = 16;
                texti.FontWeight = FontWeights.Bold;
                Canvas.SetLeft(texti, li.X2 - texti.Width - 4);
                Canvas.SetTop(texti, li.Y1 - 10);
                Canvas_ViewPortLeft.Children.Add(texti);
            }

            TextBlock textblock = new TextBlock();
            textblock.Width = 30;
            textblock.Padding = new Thickness(2, 0, 0, 0);
            textblock.TextAlignment = TextAlignment.Left;
            textblock.Text = AirSpeed.ToString("##0.#");
            textblock.Foreground = Brushes.White;
            textblock.Background = Brushes.Red;
            textblock.FontSize = 12;
            textblock.FontWeight = FontWeights.Bold;
            double offset = (from - AirSpeed) / 25 * (rectangle.Height - 20) - rectangle.Height / 2 + 10;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetLeft(textblock, 0);
            Canvas_ViewPortLeft.Children.Add(textblock);

        }
        private void RedrawOthersInfoText()
        {
            BorderTextLabel airspeedlabel = new BorderTextLabel();
            airspeedlabel.Stroke = Brushes.DimGray;
            airspeedlabel.Foreground = Brushes.White;
            airspeedlabel.FontSize = 14;
            airspeedlabel.FontWeight = FontWeights.Bold;
            airspeedlabel.Text = "空速  " + AirSpeed.ToString("0.0");
            Canvas.SetLeft(airspeedlabel, 4);
            Canvas.SetTop(airspeedlabel, 80);
            Canvas_ViewPortLeft.Children.Add(airspeedlabel);

            BorderTextLabel groundspeedlabel = new BorderTextLabel();
            groundspeedlabel.Stroke = Brushes.DimGray;
            groundspeedlabel.Foreground = Brushes.White;
            groundspeedlabel.FontSize = 14;
            groundspeedlabel.FontWeight = FontWeights.Bold;
            groundspeedlabel.Text = "地速  " + GroundSpeed.ToString("0.0");
            Canvas.SetLeft(groundspeedlabel, 4);
            Canvas.SetTop(groundspeedlabel, 100);
            Canvas_ViewPortLeft.Children.Add(groundspeedlabel);

            BorderTextLabel batteryabel = new BorderTextLabel();
            batteryabel.Stroke = Brushes.DimGray;
            batteryabel.Foreground = Brushes.White;
            batteryabel.FontSize = 14;
            batteryabel.FontWeight = FontWeights.Bold;
            batteryabel.Text = string.Format("电池  {0} v  {1} A  {2}%", Voltage.ToString("0.00"), Galvanic.ToString("0.0"), BatteryPercent * 100);
            Canvas.SetLeft(batteryabel, 10);
            Canvas.SetTop(batteryabel, 130);
            Canvas_ViewPortLeft.Children.Add(batteryabel);

            if (HasEKF)
            {
                BorderTextLabel ekflabel = new BorderTextLabel();
                ekflabel.Stroke = Brushes.DimGray;
                ekflabel.Foreground = Brushes.White;
                ekflabel.FontSize = 14;
                ekflabel.FontWeight = FontWeights.Bold;
                ekflabel.Text = "EKF";
                Canvas.SetLeft(ekflabel, 200);
                Canvas.SetTop(ekflabel, 130);
                Canvas_ViewPortLeft.Children.Add(ekflabel);
            }
            if (HasVibe)
            {
                BorderTextLabel vibelabel = new BorderTextLabel();
                vibelabel.Stroke = Brushes.DimGray;
                vibelabel.Foreground = Brushes.Red;
                vibelabel.FontSize = 14;
                vibelabel.FontWeight = FontWeights.Bold;
                vibelabel.Text = "Vibe";
                Canvas.SetLeft(vibelabel, 250);
                Canvas.SetTop(vibelabel, 130);
                Canvas_ViewPortLeft.Children.Add(vibelabel);
            }

            BorderTextLabel gpslabel = new BorderTextLabel();
            gpslabel.Stroke = Brushes.DimGray;
            gpslabel.Foreground = Brushes.Red;
            gpslabel.FontSize = 14;
            gpslabel.FontWeight = FontWeights.Bold;
            string s = HasGPS ? "有GPS" : "无GPS";
            gpslabel.Text = "GPS: " + s;
            Canvas.SetLeft(gpslabel, 300);
            Canvas.SetTop(gpslabel, 130);
            Canvas_ViewPortLeft.Children.Add(gpslabel);

            BorderTextLabel flytimelabel = new BorderTextLabel();
            flytimelabel.Stroke = Brushes.DimGray;
            flytimelabel.Foreground = Brushes.White;
            flytimelabel.FontSize = 14;
            flytimelabel.FontWeight = FontWeights.Bold;
            flytimelabel.Text = new TimeSpan(FlyTime).ToString(@"hh\:mm\:ss");
            Canvas.SetLeft(flytimelabel, -80);
            Canvas.SetTop(flytimelabel, -110);
            Canvas_ViewPortRight.Children.Add(flytimelabel);


        }
        private void RedrawOthers()
        {
            Canvas_ViewPortLeft.Children.Clear();
            Canvas_ViewPortRight.Children.Clear();

            RedrawHeight();
            RedrawSpeed();
            RedrawOthersInfoText();
        }

        #endregion


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RedrawYaw();
            RedrawRoll();
            RedrawPitch();

            RedrawOthers();//绘制高度、速度等
        }

    }
}
