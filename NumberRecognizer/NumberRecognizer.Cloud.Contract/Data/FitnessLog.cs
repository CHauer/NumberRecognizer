using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    public class FitnessLog
    {
        public IList<double> FitnessTrend { get; set; }

        public Dictionary<string, IList<double>> PatternTrends { get; set; }

        public Dictionary<string, double>  FinalPatternFittness { get; set; }

    }
}
