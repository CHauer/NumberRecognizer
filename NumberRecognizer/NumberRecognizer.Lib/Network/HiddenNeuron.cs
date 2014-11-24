using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberRecognizer.Lib.Network
{
    public class HiddenNeuron : INeuron
    {
        public HiddenNeuron()
        {
            throw new System.NotImplementedException();
        }
    
        public double ActivationValue
        {
            get { throw new NotImplementedException(); }
        }

        public List<WeightedLink> InputLayer
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
