﻿//-----------------------------------------------------------------------
// <copyright file="ConnectedComponent.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Connected Component.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognition.Labeling
{
    using System.Collections.Generic;
    using NumberRecognizer.Labeling;

    /// <summary>
    /// Connected Component.
    /// </summary>
    public class ConnectedComponent
    {
        /// <summary>
        /// The points.
        /// </summary>
        private List<ConnectedComponentPoint> points = new List<ConnectedComponentPoint>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedComponent"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        public ConnectedComponent(int label)
        {
            this.Label = label;
        }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public List<ConnectedComponentPoint> ComponentPoints
        {
            get { return this.points; }
            set { this.points = value; }
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public int Label
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the minimum bounding rectangle.
        /// </summary>
        /// <value>
        /// The minimum bounding rectangle.
        /// </value>
        public MinimumBoundingRectangle MinBoundingRect
        {
            get
            {
                return new MinimumBoundingRectangle(this.points);
            }
        }

        /// <summary>
        /// Gets or sets the pixels.
        /// </summary>
        /// <value>
        /// The pixels.
        /// </value>
        public double[,] Pixels { get; set; }

        /// <summary>
        /// Gets or sets the scaled pixels.
        /// </summary>
        /// <value>
        /// The scaled pixels.
        /// </value>
        public double[,] ScaledPixels { get; set; }

        /// <summary>
        /// Gets or sets the bytes.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Gets or sets the scaled bytes.
        /// </summary>
        /// <value>
        /// The scaled bytes.
        /// </value>
        public byte[] ScaledBytes { get; set; }
    }
}
