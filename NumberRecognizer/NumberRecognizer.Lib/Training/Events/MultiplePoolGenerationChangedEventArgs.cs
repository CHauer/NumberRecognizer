//-----------------------------------------------------------------------
// <copyright file="MultiplePoolGenerationChangedEventArgs.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>MultiplePoolGenerationChangedEventArgs for EventHandler.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Training.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NumberRecognizer.Lib.Network;

    /// <summary>
    /// MultiplePoolGenerationChangedEventArgs for EventHandler.
    /// </summary>
    public class MultipleGenPoolGenerationChangedEventArgs : GenerationChangedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleGenPoolGenerationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <param name="network">The network.</param>
        /// <param name="patternFitness">The pattern fitness.</param>
        /// <param name="multipleGenPoolIdentifier">The multiple gen pool identifier.</param>
        public MultipleGenPoolGenerationChangedEventArgs(int generation, PatternRecognitionNetwork network, Dictionary<string, double> patternFitness, int multipleGenPoolIdentifier)
            : base(generation, network, patternFitness)
        {
            this.MultipleGenPoolIdentifier = multipleGenPoolIdentifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleGenPoolGenerationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="args">The <see cref="GenerationChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="multipleGenPoolIdentifier">The multiple gen pool identifier.</param>
        public MultipleGenPoolGenerationChangedEventArgs(GenerationChangedEventArgs args, int multipleGenPoolIdentifier)
            : base(args.Generation, args.CurrentFittestNetwork, args.PatternFitness)
        {
            this.MultipleGenPoolIdentifier = multipleGenPoolIdentifier;
        }

        /// <summary>
        /// Gets the multiple gen pool identifier.
        /// </summary>
        /// <value>
        /// The multiple gen pool identifier.
        /// </value>
        public int MultipleGenPoolIdentifier { get; private set; }
    }
}
