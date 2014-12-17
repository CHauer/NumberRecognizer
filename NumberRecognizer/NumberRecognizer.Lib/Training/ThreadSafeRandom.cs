//-----------------------------------------------------------------------
// <copyright file="ThreadSafeRandom.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>ThreadSafeRandom - Random Helper.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Training
{
    using System;

    /// <summary>
    /// ThreadSafeRandom Class.
    /// </summary>
    public class ThreadSafeRandom
    {
        /// <summary>
        /// The Global
        /// </summary>
        private static readonly Random Global = new Random();

        /// <summary>
        /// The local
        /// </summary>
        [ThreadStatic]
        private static Random local;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeRandom"/> class.
        /// </summary>
        public ThreadSafeRandom()
        {
            if (local != null)
            {
                return;
            }

            int seed;

            lock (Global)
            {
                seed = Global.Next();
            }

            local = new Random(seed);
        }

        /// <summary>
        /// Next random value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>Next Integer Random.</returns>
        public int Next(int maxValue)
        {
            return local.Next(maxValue);
        }

        /// <summary>
        /// Next random double.
        /// </summary>
        /// <returns>The next Double random.</returns>
        public double NextDouble()
        {
            return local.NextDouble();
        }

        /// <summary>
        /// Next random gaussian.
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        /// <returns>The next Gaussian random.</returns>
        public double NextGaussian(double mean, double standardDeviation)
        {
            // Box-Muller
            double r1 = NextDouble();
            double r2 = NextDouble();

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(r1)) * Math.Sin(2.0 * Math.PI * r2);

            return mean + (standardDeviation * randStdNormal);
        }
    }
}