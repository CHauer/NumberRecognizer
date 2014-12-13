//-----------------------------------------------------------------------
// <copyright file="inkCanvasRT.cs" company="FH Wr.Neustadt">
//     Copyright (imgCol) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Control like InkCanvas for RT.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.App.Control
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using NumberRecognizer.App.Help;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Input;
    using Windows.UI.Input.Inking;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Shapes;
    using NumberRecognizer.Labeling;

    /// <summary>
    /// InkCanvas for RT
    /// </summary>
    public class InkCanvasRT : Canvas
    {
        /// <summary>
        /// The ink manager
        /// </summary>
        private InkManager inkManager;

        /// <summary>
        /// The pen identifier
        /// </summary>
        private uint penID;

        /// <summary>
        /// The previous contact point
        /// </summary>
        private Point previousContactPoint;

        /// <summary>
        /// The current contact point
        /// </summary>
        private Windows.Foundation.Point currentContactPoint;

        /// <summary>
        /// The connected component labeling
        /// </summary>
        private ConnectedComponentLabeling connectedComponentLabeling;

        /// <summary>
        /// Initializes a new instance of the <see cref="InkCanvasRT"/> class.
        /// </summary>
        public InkCanvasRT()
        {
            this.inkManager = new InkManager();
            this.ForegroundColor = Colors.Black;
            this.BackgroundColor = Colors.White;
            this.Background = new SolidColorBrush(this.BackgroundColor);
            this.Margin = new Thickness() { Bottom = 2, Left = 2, Right = 2, Top = 2 };
            this.StrokeThickness = 4.0;
            this.PointerPressed += this.InkCanvasRT_PointerPressed;
            this.PointerReleased += this.InkCanvasRT_PointerReleased;
            this.PointerExited += this.InkCanvasRT_PointerReleased;
            this.PointerMoved += this.InkCanvasRT_PointerMoved;
            this.IsRightTapEnabled = true;
            this.RightTapped += this.InkCanvasRT_RightTapped;
        }

        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>
        /// The color of the foreground.
        /// </value>
        public Color ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the connected component labeling.
        /// </summary>
        /// <value>
        /// The connected component labeling.
        /// </value>
        public ConnectedComponentLabeling ConnectedComponentLabeling
        {
            get
            {
                return this.connectedComponentLabeling;
            }

            set
            {
                this.connectedComponentLabeling = value;
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public double StrokeThickness
        {
            get;
            set;
        }

        /// <summary>
        /// Distance between p1 and p2.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The distance between p1 and p2.</returns>
        private static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        /// <summary>
        /// Handles the RightTapped event of the inkCanvasRT control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RightTappedRoutedEventArgs"/> instance containing the event data.</param>
        private void InkCanvasRT_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            this.Children.Clear();
            this.inkManager = new InkManager();
        }

        /// <summary>
        /// Handles the PointerPressed event of the inkCanvasRT control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void InkCanvasRT_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(this);
            this.previousContactPoint = pointerPoint.Position;
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen ||
                    (pointerDevType == PointerDeviceType.Mouse &&
                    pointerPoint.Properties.IsLeftButtonPressed))
            {
                this.inkManager.ProcessPointerDown(pointerPoint);
                this.penID = pointerPoint.PointerId;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the PointerMoved event of the inkCanvasRT control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void InkCanvasRT_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == this.penID)
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this);
                this.currentContactPoint = pointerPoint.Position;

                if (Distance(this.previousContactPoint, this.currentContactPoint) > this.StrokeThickness)
                {
                    Polyline polyLine = new Polyline()
                    {
                        Points = { this.previousContactPoint, this.currentContactPoint },
                        StrokeThickness = this.StrokeThickness,
                        Stroke = new SolidColorBrush(this.ForegroundColor)
                    };

                    this.previousContactPoint = this.currentContactPoint;
                    this.Children.Add(polyLine);
                }

                this.inkManager.ProcessPointerUpdate(pointerPoint);
            }
        }

        /// <summary>
        /// Handles the PointerReleased event of the inkCanvasRT control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private async void InkCanvasRT_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == this.penID)
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this);
                this.inkManager.ProcessPointerUp(pointerPoint);
            }

            this.penID = 0;

            this.RefreshCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Renders the stroke.
        /// </summary>
        /// <param name="stroke">The stroke.</param>
        private void RenderStroke(InkStroke stroke)
        {
            var renderingStrokes = stroke.GetRenderingSegments();
            var pathFigure = new PathFigure() { StartPoint = renderingStrokes.First().Position };
            var path = new Path()
            {
                Data = new PathGeometry()
                {
                    Figures = new PathFigureCollection() { pathFigure }
                },
                StrokeThickness = this.StrokeThickness,
                Stroke = new SolidColorBrush(this.ForegroundColor),
                UseLayoutRounding = true
            };

            foreach (var renderStroke in renderingStrokes)
            {
                pathFigure.Segments.Add(new BezierSegment()
                {
                    Point1 = renderStroke.BezierControlPoint1,
                    Point2 = renderStroke.BezierControlPoint2,
                    Point3 = renderStroke.Position
                });
            }

            this.Children.Add(path);
        }

        /// <summary>
        /// Renders the strokes.
        /// </summary>
        private void RenderStrokes()
        {
            foreach (var stroke in this.inkManager.GetStrokes())
            {
                this.RenderStroke(stroke);
            }
        }

        /// <summary>
        /// Refreshes the canvas.
        /// </summary>
        private void RefreshCanvas()
        {
            this.Children.Clear();

            this.RenderStrokes();
        }
    }
}