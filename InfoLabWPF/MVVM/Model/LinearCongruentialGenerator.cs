using System;
using System.Collections.Generic;

namespace InfoLabWPF.MVVM.Model
{
    public class LinearCongruentialGenerator
    {
        private readonly int _modulus;
        private readonly int _multiplier;
        private readonly int _increment;
        private int _currentSeed;
        private readonly int _initialSeed;
        
        public LinearCongruentialGenerator(int modulus, int multiplier, int increment, int seed)
        {
            _modulus = modulus;
            _multiplier = multiplier;
            _increment = increment;
            _currentSeed = seed;
            _initialSeed = seed;
        }
        
        public int Next()
        {
            _currentSeed = (_multiplier * _currentSeed + _increment) % _modulus;
            return _currentSeed;
        }
        
        public IEnumerable<int> GenerateSequence(int count)
        {
            var sequence = new List<int>();
            for (int i = 0; i < count; i++)
            {
                sequence.Add(Next());
            }
            return sequence;
        }
        
        public int FindPeriod()
        {
            var firstSeed = _initialSeed;
            var currentSeed = firstSeed;
            HashSet<int> seenSeeds = new HashSet<int>();
            int steps = 0;

            do
            {
                currentSeed = Next();
                steps++;

                if (seenSeeds.Contains(currentSeed))
                {
                    return steps - 1; 
                }

                seenSeeds.Add(currentSeed);
            } while (steps < _modulus);

            return -1; 
        }

    }
}