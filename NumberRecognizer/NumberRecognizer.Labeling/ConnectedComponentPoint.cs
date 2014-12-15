//-----------------------------------------------------------------------
// <copyright file="ConnectedComponentPoint.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Connected Component Point.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.Labeling
{
    /// <summary>
    /// Connected Component Point.
    /// </summary>
    public class ConnectedComponentPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedComponentPoint"/> class.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public ConnectedComponentPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x-coordinate.
        /// </value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y-coordinate.
        /// </value>
        public int Y { get; set; }
    }
}
