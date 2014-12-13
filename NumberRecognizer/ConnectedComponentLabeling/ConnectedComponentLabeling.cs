//-----------------------------------------------------------------------
// <copyright file="ConnectedComponentLabeling.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Connected Component Labeling with Two-Pass Algorithm.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.ConnectedComponentLabeling
{
    using System;
    using System.Collections.Generic;
    using Windows.Foundation;

    /// <summary>
    /// Connected Component Labeling with Two-Pass Algorithm.
    /// </summary>
    public class ConnectedComponentLabeling
    {
        /// <summary>
        /// The labeled.
        /// </summary>
        private int[,] labeled;

        /// <summary>
        /// The next label.
        /// </summary>
        private int nextLabel = 1;

        /// <summary>
        /// The next region.
        /// </summary>
        private int nextRegion = 1;

        /// <summary>
        /// The label statistics.
        /// </summary>
        private Dictionary<int, List<Point>> statistics;

        /// <summary>
        /// The minimum bounding rectangles.
        /// </summary>
        private Dictionary<int, MinimumBoundingRectangle> minimumBoundingRectangles;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedComponentLabeling" /> class.
        /// </summary>
        /// <param name="unlabeled">The unlabeled.</param>
        /// <param name="background">The background.</param>
        public ConnectedComponentLabeling(double[,] unlabeled, double background)
        {
            int length = unlabeled.Length;
            int height = unlabeled.GetLength(0);
            int width = unlabeled.GetLength(1);

            this.labeled = new int[height, width];

            int[] parents = new int[length];
            int[] labels = new int[length];

            /* FIRST PASS */
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (unlabeled[y, x] != background)
                    {
                        int label = 0;
                        int top = y - 1, left = x - 1;
                        bool isLeftConnected, isTopConnected;

                        /* if current pixel is connected to the left pixel */
                        if (isLeftConnected = left >= 0 && unlabeled[y, x] == unlabeled[y, left])
                        {
                            label = this.labeled[y, left];
                        }

                        /* if current pixel is connected to the top pixel*/
                        if (isTopConnected = top >= 0 && unlabeled[y, x] == unlabeled[top, x])
                        {
                            label = this.labeled[top, x];
                        }

                        /* if pixel not connected to the left and not connected to the top, then labeled with next region */
                        if (!isLeftConnected && !isTopConnected)
                        {
                            label = this.nextRegion++;
                        }

                        this.labeled[y, x] = label;

                        /* if pixel is connected to the left, but with different labeled, then do union */
                        if (isLeftConnected && this.labeled[y, x] != this.labeled[y, left])
                        {
                            this.Union(label, this.labeled[y, left], parents);
                        }

                        /* if pixel is connected to the top, but with different labeled, then do union */
                        if (isTopConnected && this.labeled[y, x] != this.labeled[top, x])
                        {
                            this.Union(label, this.labeled[top, x], parents);
                        }
                    }
                }
            }

            /* SECOND PASS */
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (unlabeled[y, x] != background)
                    {
                        this.labeled[y, x] = this.Find(this.labeled[y, x], parents, labels);
                    }
                }
            }

            /* LABEL STATISTICS */
            this.statistics = new Dictionary<int, List<Point>>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int label = this.labeled[y, x];
                    if (label != background)
                    {
                        if (!this.statistics.ContainsKey(label))
                        {
                            this.statistics.Add(label, new List<Point>());
                        }

                        this.statistics[label].Add(new Point(x, y));
                    }
                }
            }

            /* MINIMUM BOUNDING RECTANGLES */
            this.minimumBoundingRectangles = new Dictionary<int, MinimumBoundingRectangle>();
            foreach (KeyValuePair<int, List<Point>> label in this.statistics)
            {
                this.minimumBoundingRectangles.Add(label.Key, new MinimumBoundingRectangle(label.Value));
            }
        }

        /// <summary>
        /// Gets the minimum bounding rectangles.
        /// </summary>
        /// <value>
        /// The minimum bounding rectangles.
        /// </value>
        public Dictionary<int, MinimumBoundingRectangle> MinimumBoundingRectangles
        {
            get { return this.minimumBoundingRectangles; }
        }

        /// <summary>
        /// Gets the labeled.
        /// </summary>
        /// <value>
        /// The labeled.
        /// </value>
        public int[,] Labeled
        {
            get { return this.labeled; }
        }

        /// <summary>
        /// Gets the label count.
        /// </summary>
        /// <value>
        /// The label count.
        /// </value>
        public int Count
        {
            get
            {
                return this.statistics.Count;
            }
        }

        /// <summary>
        /// Finds the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="parents">The parents.</param>
        /// <param name="labels">The labels.</param>
        /// <returns>The root label.</returns>
        private int Find(int label, int[] parents, int[] labels)
        {
            while (parents[label] > 0)
            {
                label = parents[label];
            }

            if (labels[label] == 0)
            {
                labels[label] = this.nextLabel++;
            }

            return labels[label];
        }

        /// <summary>
        /// Unions the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="parents">The parents.</param>
        private void Union(int label, int parent, int[] parents)
        {
            while (parents[label] > 0)
            {
                label = parents[label];
            }

            while (parents[parent] > 0)
            {
                parent = parents[parent];
            }

            if (label != parent)
            {
                if (label < parent)
                {
                    parents[label] = parent;
                }
                else
                {
                    parents[parent] = label;
                }
            }
        }
    }
}
