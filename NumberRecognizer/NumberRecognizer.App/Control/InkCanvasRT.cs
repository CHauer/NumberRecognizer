//-----------------------------------------------------------------------
// <copyright file="inkCanvasRT.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Control like InkCanvas for RT.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.Control
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NumberRecognition.Labeling;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Input;
    using Windows.UI.Input.Inking;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// InkCanvas for RT.
    /// </summary>
    public class InkCanvasRT : Canvas
    {
        /// <summary>
        /// The ink manager.
        /// </summary>
        private InkManager inkManager;

        /// <summary>
        /// The pen identifier.
        /// </summary>
        private uint penID;

        /// <summary>
        /// The previous contact point.
        /// </summary>
        private Point previousContactPoint;

        /// <summary>
        /// The current contact point.
        /// </summary>
        private Point currentContactPoint;

        /// <summary>
        /// The connected component labeling.
        /// </summary>
        private ConnectedComponentLabeling labeling = new ConnectedComponentLabeling();

        /// <summary>
        /// Initializes a new instance of the <see cref="InkCanvasRT"/> class.
        /// </summary>
        public InkCanvasRT()
        {
            this.inkManager = new InkManager();
            this.ForegroundColor = Colors.Black;
            this.BackgroundColor = Colors.White;
            this.Background = new SolidColorBrush(this.BackgroundColor);
            this.StrokeThickness = 5.0;
            this.Margin = new Thickness(1);
            this.PointerPressed += this.InkCanvasRT_PointerPressed;
            this.PointerReleased += this.InkCanvasRT_PointerReleased;
            this.PointerExited += this.InkCanvasRT_PointerReleased;
            this.PointerMoved += this.InkCanvasRT_PointerMoved;
            this.IsRightTapEnabled = true;
            this.RightTapped += this.InkCanvasRT_RightTapped;
            this.Labeling = new ConnectedComponentLabeling();
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
        public ConnectedComponentLabeling Labeling { get; set; }

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
        /// Refreshes the canvas.
        /// </summary>
        public void RefreshCanvas()
        {
            this.Children.Clear();
            this.RenderStrokes();
        }

        /// <summary>
        /// Distance to the specified previous point.
        /// </summary>
        /// <param name="previousPoint">The previous point.</param>
        /// <param name="currentPoint">The current point.</param>
        /// <returns>The Distance to the specified previous point.</returns>
        private static double Distance(Point previousPoint, Point currentPoint)
        {
            return Math.Sqrt(Math.Pow(currentPoint.X - previousPoint.X, 2) + Math.Pow(currentPoint.Y - previousPoint.Y, 2));
        }

        /// <summary>
        /// Handles the RightTapped event of the inkCanvasRT control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RightTappedRoutedEventArgs"/> instance containing the event data.</param>
        private void InkCanvasRT_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ClearInk();
        }

        /// <summary>
        /// Clears the ink.
        /// </summary>
        public void ClearInk()
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

                if (Distance(this.previousContactPoint, this.currentContactPoint) > 2.0)
                {
                    Line line = new Line()
                    {
                        X1 = this.previousContactPoint.X,
                        Y1 = this.previousContactPoint.Y,
                        X2 = this.currentContactPoint.X,
                        Y2 = this.currentContactPoint.Y,
                        StrokeThickness = this.StrokeThickness,
                        Stroke = new SolidColorBrush(this.ForegroundColor)
                    };

                    this.previousContactPoint = this.currentContactPoint;
                    this.Children.Add(line);
                }

                this.inkManager.ProcessPointerUpdate(pointerPoint);
            }
        }

        /// <summary>
        /// Handles the PointerReleased event of the inkCanvasRT control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void InkCanvasRT_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == this.penID)
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this);
                this.inkManager.ProcessPointerUp(pointerPoint);
            }

            this.penID = 0;

            this.RenderStrokes();
            e.Handled = true;
        }

        /// <summary>
        /// Renders the stroke.
        /// </summary>
        /// <param name="inkStroke">The stroke.</param>
        private void RenderStroke(InkStroke inkStroke)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            IReadOnlyList<InkStrokeRenderingSegment> inkStrokeRenderingSegments = inkStroke.GetRenderingSegments();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = inkStrokeRenderingSegments.First().BezierControlPoint1;
            foreach (InkStrokeRenderingSegment inkStrokeRenderingSegment in inkStrokeRenderingSegments)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point1 = inkStrokeRenderingSegment.BezierControlPoint1;
                bezierSegment.Point2 = inkStrokeRenderingSegment.BezierControlPoint2;
                bezierSegment.Point3 = inkStrokeRenderingSegment.Position;
                pathSegments.Add(bezierSegment);
            }

            pathFigure.Segments = pathSegments;

            PathFigureCollection pathFigures = new PathFigureCollection();
            pathFigures.Add(pathFigure);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = pathFigures;

            Path path = new Path();
            path.Stroke = new SolidColorBrush(this.ForegroundColor);
            path.StrokeThickness = this.StrokeThickness * 2;
            path.Data = pathGeometry;

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
    }
}