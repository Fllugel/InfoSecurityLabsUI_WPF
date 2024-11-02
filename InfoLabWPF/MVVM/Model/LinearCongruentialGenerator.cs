using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace InfoLabWPF.MVVM.Model
{
    public class LinearCongruentialGenerator
    {
        private readonly uint _modulus;
        private readonly uint _multiplier;
        private readonly uint _increment;
        private uint _currentSeed;
        private readonly uint _initialSeed;

        public LinearCongruentialGenerator(uint modulus, uint multiplier, uint increment, uint seed)
        {
            _modulus = modulus;
            _multiplier = multiplier;
            _increment = increment;
            _currentSeed = seed;
            _initialSeed = seed;
        }

        public uint Next()
        {
            _currentSeed = (_multiplier * _currentSeed + _increment) % _modulus;
            return _currentSeed;
        }

        public IEnumerable<uint> GenerateSequence(uint count)
        {
            var sequence = new List<uint>();
            for (uint i = 0; i < count; i++)
            {
                sequence.Add(Next());
            }
            return sequence;
        }

        public int FindPeriod()
        {
            var firstSeed = _initialSeed;
            var currentSeed = firstSeed;
            HashSet<uint> seenSeeds = new HashSet<uint>();
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
