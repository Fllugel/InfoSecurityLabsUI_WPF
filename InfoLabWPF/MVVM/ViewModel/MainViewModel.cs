using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand Lab1ViewCommand { get; }
        public RelayCommand Lab2ViewCommand { get; }
        public RelayCommand Lab3ViewCommand { get; }
        public RelayCommand Lab4ViewCommand { get; }
        public RelayCommand Lab5ViewCommand { get; }
        

        public Lab1ViewModel Lab1VM { get; } 
        public Lab2ViewModel Lab2VM { get; }
        public Lab3ViewModel Lab3VM { get; }
        public Lab4ViewModel Lab4VM { get; }
        public Lab5ViewModel Lab5VM { get; }
        
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public MainViewModel()
        {
            Lab1VM = new Lab1ViewModel();
            Lab2VM = new Lab2ViewModel();
            Lab3VM = new Lab3ViewModel();
            Lab4VM = new Lab4ViewModel();
            Lab5VM = new Lab5ViewModel();
            
            CurrentView = Lab1VM;
            
            Lab1ViewCommand = new RelayCommand(() =>
            {
                CurrentView = Lab1VM;
            });
            
            Lab2ViewCommand = new RelayCommand(() =>
            {
                CurrentView = Lab2VM;
            });
            
            Lab3ViewCommand = new RelayCommand(() =>
            {
                CurrentView = Lab3VM;
            });
            
            Lab4ViewCommand = new RelayCommand(() =>
            {
                CurrentView = Lab4VM;
            });
            
            Lab5ViewCommand = new RelayCommand(() =>
            {
                CurrentView = Lab5VM;
            });
        }
    }
}