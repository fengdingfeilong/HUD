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
            Canvas.SetLeft(textblock, -7 * textblock.Text.Count() / 2);
            textblock.RenderTransform = new RotateTransform(angle, 7 * textblock.Text.Count() / 2, cycleR + 20);
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
            double cycleR = Grid_Virwport.ActualWidth / 4;
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
            if (pitch > Math.PI / 4) pitch = Math.PI / 4;
            if (pitch < -Math.PI / 4) pitch = -Math.PI / 4;
            offset = 0.5 * (1 + Math.Tan(pitch));
            bkbrush.GradientStops[1].Offset = offset;
            bkbrush.GradientStops[2].Offset = offset;

            DrawRollPitchCycle();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RedrawYaw();
            RedrawRoll();
            RedrawPitch();
        }

    }
}
