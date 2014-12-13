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
    public class GenerationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <param name="network">The network.</param>
        /// <param name="patternFitness">The pattern fitness.</param>
        public GenerationChangedEventArgs(int generation, PatternRecognitionNetwork network, Dictionary<string, double> patternFitness)
        {
            this.Generation = generation;
            this.CurrentFittestNetwork = network;
            this.PatternFitness = patternFitness;
        }

        /// <summary>
        /// Gets the generation.
        /// </summary>
        /// <value>
        /// The generation.
        /// </value>
        public int Generation { get; private set; }

        /// <summary>
        /// Gets the current fittest network.
        /// </summary>
        /// <value>
        /// The current fittest network.
        /// </value>
        public PatternRecognitionNetwork CurrentFittestNetwork { get; private set; }

        /// <summary>
        /// Gets the current fittest network.
        /// </summary>
        /// <value>
        /// The current fittest network.
        /// </value>
        public Dictionary<string, double> PatternFitness{ get; private set; }
    }
}
