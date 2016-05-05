using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HUD
{
    public class BorderTextLabel : System.Windows.Controls.Label
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("TextProperty", typeof(string), typeof(BorderTextLabel), new FrameworkPropertyMetadata(string.Empty));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(BorderTextLabel), new FrameworkPropertyMetadata(Brushes.Black));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(BorderTextLabel), new FrameworkPropertyMetadata((double)1));
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            FormattedText formattedText = new FormattedText(this.Text, CultureInfo.CurrentCulture, FlowDirection, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, this.Foreground);
            if (double.IsNaN(this.Width))
                this.Width = formattedText.Width;
            if (double.IsNaN(this.Height))
                this.Height = formattedText.Height;
            var textgeometry = formattedText.BuildGeometry(new Point(0, 0));
            drawingContext.DrawGeometry(this.Foreground, new Pen(Stroke, StrokeThickness), textgeometry);
        }

    }
}
