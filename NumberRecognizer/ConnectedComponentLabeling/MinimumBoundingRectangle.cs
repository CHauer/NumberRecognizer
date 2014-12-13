//-----------------------------------------------------------------------
// <copyright file="MinimumBoundingRectangle.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Minimum Bounding Rectangle.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.Labeling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// Minimum Bounding Rectangle.
    /// </summary>
    public class MinimumBoundingRectangle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinimumBoundingRectangle"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public MinimumBoundingRectangle(List<LabelingPoint> points)
        {
            this.Left = points.Min(p => p.X);
            this.Top = points.Min(p => p.Y);
            this.Width = points.Max(p => p.X) - this.Left;
            this.Height = points.Max(p => p.Y) - this.Top;
            this.Size = Math.Max(this.Width, this.Height);
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>
        /// The top most y-coordinate.
        /// </value>
        public double Top { get; private set; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left most x-coordinate.
        /// </value>
        public double Left
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size max(width, height).
        /// </value>
        public double Size
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the padding left.
        /// </summary>
        /// <value>
        /// The padding left.
        /// </value>
        public double PadLeft
        {
            get
            {
                return Math.Floor((this.Size - this.Width) / 2);
            }
        }

        /// <summary>
        /// Gets the padding top.
        /// </summary>
        /// <value>
        /// The padding top.
        /// </value>
        public double PadTop
        {
            get
            {
                return Math.Floor((this.Size - this.Height) / 2);
            }
        }
    }
}
