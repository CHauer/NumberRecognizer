using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberRecognizer.Labeling
{
    public class LabelingPoint
    {
       
        /// <summary>
        /// Initializes a new instance of the <see cref="LabelingPoint"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public LabelingPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y { get; set; }

    }
}
