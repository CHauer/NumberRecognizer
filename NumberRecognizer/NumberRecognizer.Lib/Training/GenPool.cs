using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training
{
    /// <summary>
    /// Represents a GenPool with a variable generation count 
    /// and a variable population size.
    /// Offers functionality to calculate a n generations of network populations.
    /// </summary>
    public class GenPool
    {
        private List<string> Patterns;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenPool" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        public GenPool(IEnumerable<TrainingImage> trainingData, TrainingParameter parameter)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the result networks.
        /// </summary>
        /// <value>
        /// The result networks.
        /// </value>
        public ICollection<PatternRecognitionNetwork> ResultNetworks{get; private set;}

        #endregion

        public void CalculatePool()
        {

        }

    }
}
