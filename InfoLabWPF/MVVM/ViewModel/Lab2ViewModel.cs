using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab2ViewModel : INotifyPropertyChanged
    {
        private string _message = "";
        private string _encryptedMessage;
        private string _testResults;
        private string _userNotification;

        private readonly MD5 _md5;

        public Lab2ViewModel()
        {
            _md5 = new MD5();
            EncryptCommand = new RelayCommand(EncryptMessage);
            TestHashValuesCommand = new RelayCommand(TestHashValues);
            LoadInputFromFileCommand = new RelayCommand(LoadInputFromFile);
            SaveHashToFileCommand = new RelayCommand(SaveHashToFile);
        }

        public ICommand EncryptCommand { get; }
        public ICommand TestHashValuesCommand { get; }
        public ICommand LoadInputFromFileCommand { get; }
        public ICommand SaveHashToFileCommand { get; }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                UserNotification = "";  
            }
        }

        public string EncryptedMessage
        {
            get => _encryptedMessage;
            set
            {
                _encryptedMessage = value;
                OnPropertyChanged();
            }
        }

        public string TestResults
        {
            get => _testResults;
            set
            {
                _testResults = value;
                OnPropertyChanged();
            }
        }

        public string UserNotification
        {
            get => _userNotification;
            set
            {
                _userNotification = value;
                OnPropertyChanged();
            }
        }

        private void EncryptMessage()
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(Message);
            byte[] hashBytes = _md5.ComputeHash(messageBytes);
            EncryptedMessage = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

            MessageBox.Show("Message encryption was successful.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TestHashValues()
        {
            ConfigLoader configLoader = new ConfigLoader();
            var expectedValues = configLoader.LoadConfigLab2();

            var results = new StringBuilder();
            foreach (var (input, expected) in expectedValues)
            {
                byte[] hashBytes = _md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "");

                results.AppendLine($"Input: {input}");
                results.AppendLine($"Expected: {expected}");
                results.AppendLine($"Computed: {hashString}");
                results.AppendLine();
            }

            TestResults = results.ToString();
        }

        private void LoadInputFromFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load Input File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _md5.LoadInputFromFile(openFileDialog.FileName, out string inputText);
                    Message = inputText;  
                    UserNotification = "Loaded message from file. Type your own message to override."; 
                    Console.WriteLine("Input file loaded successfully.");
                }
                catch (Exception ex)
                {
                    ShowError($"Error loading input from file: {ex.Message}");
                }
            }
        }


        private void SaveHashToFile()
        {
            if (!string.IsNullOrEmpty(EncryptedMessage))
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter =
                        "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                    Title = "Save Hash"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        _md5.SaveHashToFile(saveFileDialog.FileName, EncryptedMessage);
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error saving hash to file: {ex.Message}");
                    }
                }
            }
            else
            {
                ShowError("There is no hash to save.");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}