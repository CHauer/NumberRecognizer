//-----------------------------------------------------------------------
// <copyright file="MinimumBoundingRectangle.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Minimum Bounding Rectangle.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.ConnectedComponentLabeling
{
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Foundation;

    /// <summary>
    /// Minimum Bounding Rectangle.
    /// </summary>
    public class MinimumBoundingRectangle
    {
        /// <summary>
        /// The top most y-coordinate.
        /// </summary>
        private double top;

        /// <summary>
        /// The left most x-coordinate.
        /// </summary>
        private double left;

        /// <summary>
        /// The right most x-coordinate.
        /// </summary>
        private double right;

        /// <summary>
        /// The bottom most y-coordinate.
        /// </summary>
        private double bottom;

        /// <summary>
        /// The points.
        /// </summary>
        private List<Point> points;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinimumBoundingRectangle"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public MinimumBoundingRectangle(List<Point> points)
        {
            this.points = points;
            this.left = points.Min(p => p.X);
            this.right = points.Max(p => p.X);
            this.top = points.Min(p => p.Y);
            this.bottom = points.Max(p => p.Y);
        }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public List<Point> Points
        {
            get { return this.points; }
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>
        /// The top most x-coordinate.
        /// </value>
        public double Top
        {
            get { return this.top; }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left most x-coordinate.
        /// </value>
        public double Left
        {
            get { return this.left; }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get { return this.right - this.left; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get { return this.bottom - this.top; }
        }
    }
}
