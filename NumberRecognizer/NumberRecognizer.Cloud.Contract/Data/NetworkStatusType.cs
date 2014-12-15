using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    public enum NetworkStatusType : int
    {
        /// <summary>
        /// The not started
        /// </summary>
        NotStarted = 1,

        /// <summary>
        /// The running
        /// </summary>
        Running = 2,

        /// <summary>
        /// The ready
        /// </summary>
        Ready = 4,

        /// <summary>
        /// The error
        /// </summary>
        Error = 8
    }
}
