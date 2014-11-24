using System;

namespace NumberRecognizer.Lib.Training
{

	public class ThreadSafeRandom
	{
		private static readonly Random global = new Random();

		[ThreadStatic]
		private static Random local;

		public ThreadSafeRandom()
		{
			if (local != null)
			{
				return;
			}

			int seed;

			lock (global)
			{
				seed = global.Next();
			}

			local = new Random(seed);
		}

		public int Next(int maxValue)
		{
			return local.Next(maxValue);
		}

		public double NextDouble()
		{
			return local.NextDouble();
		}

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