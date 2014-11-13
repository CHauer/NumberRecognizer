using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    public class RecognitionResultItem
    {
        public Char NumericCharacter
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Dictionary<char, decimal> Probabilities
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
