using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab1ViewModel : INotifyPropertyChanged
    {
        // Private Fields
        private LinearCongruentialGenerator _lcg;
        private GCDTest _gcdTest;
        private List<int> _sequence;
        private int _modulus;
        private int _multiplier;
        private int _increment;
        private int _seed;
        private int _sequenceCount;
        private double _piEstimate;
        private double _piBuiltInEstimate;
        private int _period;

        // Constructor
        public Lab1ViewModel()
        {
            // Initializing commands
            GenerateCommand = new RelayCommand(GenerateSequence);
            EstimatePiCommand = new RelayCommand(EstimatePi);
            EstimatePiBuiltInCommand = new RelayCommand(EstimatePiBuiltIn);
            LoadVariantDataCommand = new RelayCommand(LoadVariantData);
            FindPeriodCommand = new RelayCommand(FindPeriod);
            SaveSequenceCommand = new RelayCommand(SaveSequence);
        }

        // Commands
        public ICommand GenerateCommand { get; }
        public ICommand EstimatePiCommand { get; }
        public ICommand EstimatePiBuiltInCommand { get; }
        public ICommand LoadVariantDataCommand { get; }
        public ICommand FindPeriodCommand { get; }
        public ICommand SaveSequenceCommand { get; }

        // Properties
        public int Modulus
        {
            get => _modulus;
            set
            {
                if (value >= 0)
                {
                    _modulus = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Multiplier
        {
            get => _multiplier;
            set
            {
                if (value >= 0)
                {
                    _multiplier = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Increment
        {
            get => _increment;
            set
            {
                if (value >= 0)
                {
                    _increment = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Seed
        {
            get => _seed;
            set
            {
                if (value >= 0)
                {
                    _seed = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SequenceCount
        {
            get => _sequenceCount;
            set
            {
                if (value >= 0)
                {
                    _sequenceCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<int> Sequence
        {
            get => _sequence;
            set
            {
                _sequence = value;
                OnPropertyChanged();
            }
        }

        public double PiEstimate
        {
            get => _piEstimate;
            set
            {
                _piEstimate = value;
                OnPropertyChanged();
            }
        }

        public double PiBuiltInEstimate
        {
            get => _piBuiltInEstimate;
            set
            {
                _piBuiltInEstimate = value;
                OnPropertyChanged();
            }
        }

        public int Period
        {
            get => _period;
            set
            {
                _period = value;
                OnPropertyChanged();
            }
        }

        public double PiDeviation => PiEstimate == 0 ? 0 : Math.Abs(Math.PI - PiEstimate);
        public double PiBuiltInDeviation => PiBuiltInEstimate == 0 ? 0 : Math.Abs(Math.PI - PiBuiltInEstimate);

        // Methods
        private void GenerateSequence()
        {
            if (!IsValidGenerationInput())
            {
                MessageBox.Show("Please ensure all inputs are valid and non-negative.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _lcg = new LinearCongruentialGenerator(_modulus, _multiplier, _increment, _seed);
            Sequence = new List<int>(_lcg.GenerateSequence(_sequenceCount));
            _gcdTest = new GCDTest(Sequence);
        }

        private bool IsValidGenerationInput()
        {
            return Modulus > 0 && Multiplier > 0 && SequenceCount > 0
                   && Multiplier < Modulus && Increment < Modulus && Seed < Modulus;
        }

        private void EstimatePi()
        {
            if (_gcdTest != null)
            {
                PiEstimate = _gcdTest.EstimatePi();
                OnPropertyChanged(nameof(PiDeviation));
            }
        }

        private void EstimatePiBuiltIn()
        {
            var random = new Random();
            var generatedSequence = new List<int>();

            for (int i = 0; i < SequenceCount; i++)
            {
                generatedSequence.Add(random.Next(0, Modulus));
            }

            _gcdTest = new GCDTest(generatedSequence);
            PiBuiltInEstimate = _gcdTest.EstimatePi();
            OnPropertyChanged(nameof(PiBuiltInDeviation));
        }

        private void FindPeriod()
        {
            if (_lcg != null)
            {
                Period = _lcg.FindPeriod();
            }
        }

        private void SaveSequence()
        {
            _lcg?.SaveSequence(_sequence);
        }

        private void LoadVariantData()
        {
            Modulus = (1 << 19) - 1;
            Multiplier = 6 * 6 * 6;
            Increment = 55;
            Seed = 1024;
            SequenceCount = 1000;
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
