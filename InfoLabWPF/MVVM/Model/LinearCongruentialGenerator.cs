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

        public void SaveSequence(IEnumerable<uint> sequence)
        {
            if (sequence == null)
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Save Sequence"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (var number in sequence)
                        {
                            writer.WriteLine(number);
                        }
                    }

                    MessageBox.Show("Sequence saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
