using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<uint> _sequence;
        private uint _modulus;
        private uint _multiplier;
        private uint _increment;
        private uint _seed;
        private uint _sequenceCount;
        private double _piEstimate;
        private double _piBuiltInEstimate;
        private uint _period;

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
            _gcdTest = new GCDTest();
        }

        // Commands
        public ICommand GenerateCommand { get; }
        public ICommand EstimatePiCommand { get; }
        public ICommand EstimatePiBuiltInCommand { get; }
        public ICommand LoadVariantDataCommand { get; }
        public ICommand FindPeriodCommand { get; }
        public ICommand SaveSequenceCommand { get; }

        // Properties with string input and validation
        public string Modulus
        {
            get => _modulus.ToString();
            set
            {
                if (uint.TryParse(value, out uint result))
                {
                    _modulus = result;
                    OnPropertyChanged();
                }
                else
                {
                    ShowError("Modulus must be a positive integer and less than the maximum value of uint.");
                }
            }
        }

        public string Multiplier
        {
            get => _multiplier.ToString();
            set
            {
                if (uint.TryParse(value, out uint result))
                {
                    _multiplier = result;
                    OnPropertyChanged();
                }
                else
                {
                    ShowError("Multiplier must be a positive integer and less than the maximum value of uint.");
                }
            }
        }

        public string Increment
        {
            get => _increment.ToString();
            set
            {
                if (uint.TryParse(value, out uint result))
                {
                    _increment = result;
                    OnPropertyChanged();
                }
                else
                {
                    ShowError("Increment must be a positive integer and less than the maximum value of uint.");
                }
            }
        }

        public string Seed
        {
            get => _seed.ToString();
            set
            {
                if (uint.TryParse(value, out uint result))
                {
                    _seed = result;
                    OnPropertyChanged();
                }
                else
                {
                    ShowError("Seed must be a positive integer and less than the maximum value of uint.");
                }
            }
        }

        public string SequenceCount
        {
            get => _sequenceCount.ToString();
            set
            {
                if (uint.TryParse(value, out uint result))
                {
                    _sequenceCount = result;
                    OnPropertyChanged();
                }
                else
                {
                    ShowError("Sequence count must be a positive integer and less than the maximum value of uint.");
                }
            }
        }

        public List<uint> Sequence
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

        public string Period
        {
            get => _period.ToString();
            set
            {
                if (uint.TryParse(value, out uint result))
                {
                    _period = result;
                    OnPropertyChanged();
                }
                else
                {
                    ShowError("Period must be a positive integer and less than the maximum value of uint.");
                }
            }
        }

        public double PiDeviation => PiEstimate == 0 ? 0 : Math.Abs(Math.PI - PiEstimate);
        public double PiBuiltInDeviation => PiBuiltInEstimate == 0 ? 0 : Math.Abs(Math.PI - PiBuiltInEstimate);

        // Methods
        private void GenerateSequence()
        {
            _lcg = new LinearCongruentialGenerator(_modulus, _multiplier, _increment, _seed);
            Sequence = new List<uint>(_lcg.GenerateSequence(_sequenceCount));

            MessageBox.Show("Sequence generation was successful.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void EstimatePi()
        {
            if (_gcdTest != null)
            {
                PiEstimate = _gcdTest.EstimatePi(_sequence);
                OnPropertyChanged(nameof(PiDeviation));
            }
        }

        private void EstimatePiBuiltIn()
        {
            var random = new Random();
            var generatedSequence = new List<uint>();

            for (int i = 0; i < _sequenceCount; i++)
            {
                generatedSequence.Add((uint)random.Next(0, (int)_modulus));
            }
            PiBuiltInEstimate = _gcdTest.EstimatePi(generatedSequence);
            OnPropertyChanged(nameof(PiBuiltInDeviation));
        }

        private void FindPeriod()
        {
            if (_lcg != null)
            {
                Period = _lcg.FindPeriod().ToString();
            }
        }

        private void SaveSequence()
        {
            _lcg?.SaveSequence(_sequence);
        }

        private void LoadVariantData()
        {
            ConfigLoader configLoader = new ConfigLoader();

            Modulus = configLoader.Modulus.ToString();
            Multiplier = configLoader.Multiplier.ToString();
            Increment = configLoader.Increment.ToString();
            Seed = configLoader.Seed.ToString();
            SequenceCount = configLoader.SequenceCount.ToString();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}