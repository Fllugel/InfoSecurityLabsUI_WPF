using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab1ViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private LinearCongruentialGenerator _lcg;
        private GCDTest _gcdTest;
        private List<int> _sequence;
        private int _modulus;
        private int _multiplier;
        private int _increment;
        private int _seed;
        private int _sequenceCount;
        private double _piEstimate;
        private int _period;

        public Lab1ViewModel()
        {
            GenerateCommand = new RelayCommand(GenerateSequence);
            EstimatePiCommand = new RelayCommand(EstimatePi);
            LoadVariantDataCommand = new RelayCommand(() => LoadVariantData());
            FindPeriodCommand = new RelayCommand(FindPeriod); // Add this line
        }


        public ICommand GenerateCommand { get; }
        public ICommand EstimatePiCommand { get; }
        public ICommand LoadVariantDataCommand { get; }
        public ICommand FindPeriodCommand { get; }

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

        public int Period
        {
            get => _period;
            set
            {
                _period = value;
                OnPropertyChanged();
            }
        }


        private void GenerateSequence()
        {
            _lcg = new LinearCongruentialGenerator(_modulus, _multiplier, _increment, _seed);
            Sequence = new List<int>(_lcg.GenerateSequence(_sequenceCount));
            _gcdTest = new GCDTest(Sequence);
            OnPropertyChanged(nameof(Sequence));
        }

        private void EstimatePi()
        {
            if (_gcdTest != null)
            {
                PiEstimate = _gcdTest.EstimatePi();
                OnPropertyChanged(nameof(PiEstimate));
            }
        }
        
        private void FindPeriod()
        {
            if (_lcg != null)
            {
                Period = _lcg.FindPeriod();
                OnPropertyChanged(nameof(Period));
            }
        }


        private void LoadVariantData()
        {
            Modulus = 16;
            Multiplier = 5;
            Increment = 3;
            Seed = 7;
            SequenceCount = 100;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDataErrorInfo Implementation

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Modulus) ||
                    columnName == nameof(Multiplier) ||
                    columnName == nameof(Increment) ||
                    columnName == nameof(Seed) ||
                    columnName == nameof(SequenceCount))
                {
                    var value = GetType().GetProperty(columnName).GetValue(this);
                    if (value is int intValue)
                    {
                        return intValue >= 0 ? null : "Value must be positive or zero.";
                    }
                    else
                    {
                        return "Value must be an integer.";
                    }
                }
                return null;
            }
        }

        #endregion
    }
}
