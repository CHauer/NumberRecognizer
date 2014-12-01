using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training.Contract
{
    public interface IMutation
    {
        PatternRecognitionNetwork ExecuteMutation(PatternRecognitionNetwork network);
    }
}
