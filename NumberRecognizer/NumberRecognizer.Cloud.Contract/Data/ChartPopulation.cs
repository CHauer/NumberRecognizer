//-----------------------------------------------------------------------
// <copyright file="ChartPopulation.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>ChartPopulation DataContract.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// ChartPopulation DataContract
    /// </summary>
    public class ChartPopulation
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
         public string Name { get; set; }

         /// <summary>
         /// Gets or sets the value.
         /// </summary>
         /// <value>
         /// The value.
         /// </value>
         public double Value { get; set; }

    }
}
