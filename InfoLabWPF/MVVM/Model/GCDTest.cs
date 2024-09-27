using System;
using System.Collections.Generic;

namespace InfoLabWPF.MVVM.Model
{
    public class GCDTest
    {
        private static uint GCD(uint a, uint b)
        {
            while (b != 0)
            {
                uint temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public double EstimatePi(List<uint> sequence)
        {
            int countCoprime = 0;
            int numberOfPairs = sequence.Count / 2;

            if (sequence.Count < 2)
            {
                throw new InvalidOperationException("Sequence must contain at least two numbers.");
            }

            for (int i = 0; i < numberOfPairs; i++)
            {
                uint a = sequence[2 * i];
                uint b = sequence[2 * i + 1];

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