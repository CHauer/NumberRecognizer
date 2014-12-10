using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Contract;
using NumberRecognizer.Lib.Training.GeneticOperator;

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
            InitializeGeneticOperators();
        }

        /// <summary>
        /// Initializes the parameters with standard values.
        /// </summary>
        private void InitializeParameters()
        {
            GenPoolTrainingMode = GenPoolType.SingleGenPool;
            
            SingleGenPoolPopulationSize = 100;
            SingleGenPoolMaxGenerations = 100;

            MultipleGenPoolCount = 5;
            MultipleGenPoolMaxGenerations = 50;
            MultipleGenPoolPopulationSize = 100;

            ImageWidth = 16;
            ImageHeight = 16;
        }

        /// <summary>
        /// Initializes the genetic operators.
        /// </summary>
        private void InitializeGeneticOperators()
        {
            CrossoverInstance = new UniformCrossover();

            MutationInstances = new List<IMutation>() { 
                new GaussMutation(){ MinNetworkFitness = 0.95},
                new UniformMutation(){ MinNetworkFitness = 0.0}
            };

            SelectionInstance = new TruncationSelection()
            {
                TruncationSelectionPercentage = 0.1
            };
        }

        /// <summary>
        /// Gets or sets the gen pool training mode.
        /// </summary>
        /// <value>
        /// The gen pool training mode.
        /// </value>
        public GenPoolType GenPoolTrainingMode { get; set; }

        /// <summary>
        /// Gets or sets the size of the single gen pool population.
        /// </summary>
        /// <value>
        /// The size of the single gen pool population.
        /// </value>
        public int SingleGenPoolPopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the single gen pool generations.
        /// </summary>
        /// <value>
        /// The single gen pool generations.
        /// </value>
        public int SingleGenPoolMaxGenerations { get; set; }

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
        public int MultipleGenPoolMaxGenerations { get; set; }

        /// <summary>
        /// Gets or sets the multiple gen pool count.
        /// </summary>
        /// <value>
        /// The multiple gen pool count.
        /// </value>
        public int MultipleGenPoolCount { get; set; }

        /// <summary>
        /// Gets the multiple gen pool selection count.
        /// </summary>
        /// <value>
        /// The multiple gen pool selection count.
        /// </value>
        public int MultipleGenPoolSelectionCount
        {
            get
            {
                if (SingleGenPoolPopulationSize % MultipleGenPoolCount == 0)
                {
                    return SingleGenPoolPopulationSize / MultipleGenPoolCount;
                }

                //Standardwert
                return 20;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether elitism method is used.
        /// Fittest Network survives without genetic mutation/recombiniation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if use elitism; otherwise, <c>false</c>.
        /// </value>
        public bool UseElitism { get; set; }

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

        /// <summary>
        /// Gets or sets the mutation instance.
        /// </summary>
        /// <value>
        /// The mutation instance.
        /// </value>
        public IList<IMutation> MutationInstances { get; set; }

        /// <summary>
        /// Gets or sets the selection instance.
        /// </summary>
        /// <value>
        /// The selection instance.
        /// </value>
        public ISelection SelectionInstance { get; set; }

        /// <summary>
        /// Gets or sets the crossover instance.
        /// </summary>
        /// <value>
        /// The crossover instance.
        /// </value>
        public ICrossover CrossoverInstance { get; set; }

        /// <summary>
        /// Chooses the mutation instance based on the current network fitness.
        /// Search for the instances where the fitness is greater than the minNetworkFitness 
        /// of the instance and take the instance with the maximun minNetworkFitness.
        /// </summary>
        /// <param name="networkFitness">The network fitness.</param>
        /// <returns></returns>
        public IMutation ChooseMutation(double networkFitness)
        {
            return MutationInstances.Where(mi => networkFitness > mi.MinNetworkFitness)
                                    .OrderByDescending(mi => mi.MinNetworkFitness)
                                    .First();
        }

        /// <summary>
        /// Initializes the genetic operators.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The Genetic algorithm parts (selection, mutation, crossover)
        /// are not correct initialized.
        /// </exception>
        public void CheckGeneticOperators()
        {
            if (MutationInstances == null || MutationInstances.Count == 0 ||
                CrossoverInstance == null || SelectionInstance == null)
            {
                throw new InvalidOperationException("The Genetic algorithm parts/operators (selection, mutation, crossover) " +
                                                    "are not correct initialized.");
            }
        }

    }
}
