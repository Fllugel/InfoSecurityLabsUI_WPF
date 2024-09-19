using System;
using System.Collections.Generic;

namespace InfoLabWPF.MVVM.Model
{
    public class GCDTest
    {
        private readonly List<int> _sequence;

        public GCDTest(List<int> sequence)
        {
            _sequence = sequence;
        }
        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        
        public double EstimatePi()
        {
            int countCoprime = 0;
            int numberOfPairs = _sequence.Count / 2;

            if (_sequence.Count < 2)
            {
                throw new InvalidOperationException("Sequence must contain at least two numbers.");
            }

            for (int i = 0; i < numberOfPairs; i++)
            {
                int a = _sequence[2 * i];
                int b = _sequence[2 * i + 1];

                if (GCD(a, b) == 1)
                {
                    countCoprime++;
                }
            }
            
            double probability = (double)countCoprime / numberOfPairs;
            return Math.Sqrt(6.0 / probability);
        }
    }
}