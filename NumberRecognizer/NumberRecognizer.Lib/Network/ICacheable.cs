using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// This interface provides functionalty to
    /// reset the cached activiation value.
    /// Marks the Neuron 
    /// </summary>
    public interface ICacheable
    {

        /// <summary>
        /// Resets the cached activiation value.
        /// </summary>
        void ResetCachedValue();

    }
}
