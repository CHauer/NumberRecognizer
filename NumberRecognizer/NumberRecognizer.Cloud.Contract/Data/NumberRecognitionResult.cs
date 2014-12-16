using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class NumberRecognitionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberRecognitionResult"/> class.
        /// </summary>
        public NumberRecognitionResult()
        {
            Items = new List<NumberRecognitionResultItem>();
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        [DataMember]
        public String Number { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DataMember]
        public List<NumberRecognitionResultItem> Items { get; set; }
    }
}
