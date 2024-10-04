using System;
using System.ComponentModel;
using System.IO;
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
        private string _encryptedMessage = "";
        private string _testResults = "";
        private string _userNotification = "";
        private string _testHash = "";
        

        private readonly MD5 _md5;

        public Lab2ViewModel()
        {
            _md5 = new MD5();
            EncryptCommand = new RelayCommand(EncryptMessage);
            TestHashValuesCommand = new RelayCommand(TestHashValues);
            LoadInputFromFileCommand = new RelayCommand(LoadInputFromFile);
            SaveHashToFileCommand = new RelayCommand(SaveHashToFile);
            VerifyHashCommand = new RelayCommand(VerifyHash);
        }

        public ICommand EncryptCommand { get; }
        public ICommand TestHashValuesCommand { get; }
        public ICommand LoadInputFromFileCommand { get; }
        public ICommand SaveHashToFileCommand { get; }
        public ICommand VerifyHashCommand { get; }

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
        
        public string TestHash
        {
            get => _testHash;
            set
            {
                _testHash = value;
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
            
            Message = "";
            OnPropertyChanged(nameof(Message)); 
            
            EncryptedMessage = "";
            
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Load Input File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _md5.LoadInputFromFile(openFileDialog.FileName, out string inputText);
                    Message = inputText;  
                    UserNotification = "Loaded message from file. Type your own message to override."; 
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

        private void VerifyHash()
        {
            
            if (TestHash.Length != 32)
            {
                ShowError("The expected hash must be exactly 32 characters long.");
                return;
            }
            
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Verify Input File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    
                    _md5.LoadInputFromFile(openFileDialog.FileName, out string inputText);
                    byte[] computedHash = _md5.ComputeHash(Encoding.UTF8.GetBytes(inputText));
                    string expectedHash = TestHash; 
                    string computedHashString = BitConverter.ToString(computedHash).Replace("-", "").ToUpper();
                    string expectedHashString = expectedHash.Replace("-", "").ToUpper();

                    if (computedHashString == expectedHashString)
                    {
                        MessageBox.Show($"Hash verification successful. The file hash matches the expected hash. \nComputed Hash: {computedHashString}\nExpected Hash: {expectedHash}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Hash verification failed. The file hash does not match the expected hash. \nComputed Hash: {computedHashString}\nExpected Hash: {expectedHash}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Error verifying hash: {ex.Message}");
                }
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

