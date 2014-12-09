using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training
{
    /// <summary>
    /// 
    /// </summary>
    public class TrainingParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingParameter"/> class.
        /// </summary>
        public TrainingParameter()
        {
            InitializeParameters();
        }

        /// <summary>
        /// Initializes the parameters with standard values.
        /// </summary>
        private void InitializeParameters()
        {
            PopulationSize = 100;
            MaxGenerations = 100;
            GenPoolTrainingMode = GenPoolType.MultipleGenPool;

            MultipleGenPoolCount = 5;
            MultipleGenPoolGenerations = 50;
            MultipleGenPoolPopulationSize = 100;

            ImageWidth = 16;
            ImageHeight = 16;
        }

        /// <summary>
        /// Gets or sets the gen pool training mode.
        /// </summary>
        /// <value>
        /// The gen pool training mode.
        /// </value>
        public GenPoolType GenPoolTrainingMode { get; set; }

        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        /// <value>
        /// The size of the population.
        /// </value>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum generations.
        /// </summary>
        /// <value>
        /// The maximum generations.
        /// </value>
        public int MaxGenerations { get; set; }

        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        /// <value>
        /// The size of the population.
        /// </value>
        public int MultipleGenPoolPopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum generations.
        /// </summary>
        /// <value>
        /// The maximum generations.
        /// </value>
        public int MultipleGenPoolGenerations { get; set; }

        /// <summary>
        /// Gets or sets the multiple gen pool count.
        /// </summary>
        /// <value>
        /// The multiple gen pool count.
        /// </value>
        public int MultipleGenPoolCount { get; set; }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight { get; set; }

    }
}
