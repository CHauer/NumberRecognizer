using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training.Contract
{
    public interface ISelection
    {
        int PopulationSize { get; set; }

        IEnumerable<PatternRecognitionNetwork> ExecuteSelection(ConcurrentBag<PatternRecognitionNetwork> currentGeneration);
    }
}
