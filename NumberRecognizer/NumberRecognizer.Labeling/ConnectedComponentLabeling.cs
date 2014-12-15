//-----------------------------------------------------------------------
// <copyright file="ConnectedComponentLabeling.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Connected Component Labeling with Two-Pass Algorithm.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognition.Labeling
{
    using System;
    using System.Collections.Generic;
    using NumberRecognizer.Labeling;

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
        /// Initializes a new instance of the <see cref="ConnectedComponentLabeling"/> class.
        /// </summary>
        public ConnectedComponentLabeling()
        {
            this.ConnectedComponents = new List<ConnectedComponent>();
        }

        /// <summary>
        /// Gets the component count.
        /// </summary>
        /// <value>
        /// The component count.
        /// </value>
        public int ComponentCount
        {
            get
            {
                return this.ConnectedComponents.Count;
            }
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
        /// Gets or sets the connected components.
        /// </summary>
        /// <value>
        /// The connected components.
        /// </value>
        public List<ConnectedComponent> ConnectedComponents
        {
            get;
            set;
        }

        /// <summary>
        /// Two Pass Labeling.
        /// </summary>
        /// <param name="unlabeled">The unlabeled.</param>
        /// <param name="background">The background.</param>
        public void TwoPassLabeling(double[,] unlabeled, double background)
        {
            int length = unlabeled.Length;
            int height = unlabeled.GetLength(0);
            int width = unlabeled.GetLength(1);

            this.labeled = new int[height, width];

            int[] parents = new int[length];
            int[] labels = new int[length];

            /* FIRST PASS */
            this.nextRegion = 1;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (unlabeled[y, x] != 0)
                    {
                        int label = 0;
                        int left = x - 1;
                        int top = y - 1;
                        bool connected = false;
                        /* connected to the left */
                        if (x > 0 && unlabeled[y, left] == unlabeled[y, x])
                        {
                            label = this.labeled[y, left];
                            connected = true;
                        }

                        /* connected to the top */
                        if (y > 0 && unlabeled[top, x] == unlabeled[y, x] && (connected = false || unlabeled[top, x] < label))
                        {
                            label = this.labeled[top, x];
                            connected = true;
                        }

                        if (!connected)
                        {
                            label = this.nextRegion;
                            this.nextRegion++;
                        }

                        this.labeled[y, x] = label;
                        /* connected, but with different labels, then do union */
                        if (x > 0 && unlabeled[y, left] == unlabeled[y, x] && this.labeled[y, left] != label)
                        {
                            this.Union(label, this.labeled[y, left], parents);
                        }

                        if (y > 0 && unlabeled[top, x] == unlabeled[y, x] && this.labeled[top, x] != label)
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

            this.nextLabel--;

            /* CONNECTED COMPONENTES */
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int label = this.labeled[y, x];
                    if (label != background)
                    {
                        if (!this.ConnectedComponents.Exists(p => p.Label == label))
                        {
                            this.ConnectedComponents.Add(new ConnectedComponent(label));
                        }

                        this.ConnectedComponents.Find(p => p.Label == label).ComponentPoints.Add(new ConnectedComponentPoint(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// Finds the specified label.
        /// </summary>
        /// <param name="x">The x parameter.</param>
        /// <param name="parents">The parents.</param>
        /// <param name="labels">The labels.</param>
        /// <returns>
        /// The root label.
        /// </returns>
        private int Find(int x, int[] parents, int[] labels)
        {
            while (parents[x] > 0)
            {
                x = parents[x];
            }

            if (labels[x] == 0)
            {
                labels[x] = this.nextLabel++;
            }

            return labels[x];
        }

        /// <summary>
        /// Unions the specified label.
        /// </summary>
        /// <param name="x">The x parameter.</param>
        /// <param name="y">The y parameter.</param>
        /// <param name="parents">The parents.</param>
        private void Union(int x, int y, int[] parents)
        {
            while (parents[x] > 0)
            {
                x = parents[x];
            }

            while (parents[y] > 0)
            {
                y = parents[y];
            }

            if (x != y)
            {
                if (x < y)
                {
                    parents[x] = y;
                }
                else
                {
                    parents[y] = x;
                }
            }
        }
    }
}
