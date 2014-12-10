using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training.Events
{

    /// <summary>
    /// 
    /// </summary>
    public class MultipleGenPoolGenerationChangedEventArgs : GenerationChangedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleGenPoolGenerationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <param name="network">The network.</param>
        /// <param name="multipleGenPoolIdentifier">The multiple gen pool identifier.</param>
        public MultipleGenPoolGenerationChangedEventArgs(int generation, PatternRecognitionNetwork network, int multipleGenPoolIdentifier)
            : base(generation, network)
        {
            MultipleGenPoolIdentifier = multipleGenPoolIdentifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleGenPoolGenerationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="args">The <see cref="GenerationChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="multipleGenPoolIdentifier">The multiple gen pool identifier.</param>
        public MultipleGenPoolGenerationChangedEventArgs(GenerationChangedEventArgs args, int multipleGenPoolIdentifier)
            : base(args.Generation, args.CurrentFittestNetwork)
        {
            MultipleGenPoolIdentifier = multipleGenPoolIdentifier;
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
