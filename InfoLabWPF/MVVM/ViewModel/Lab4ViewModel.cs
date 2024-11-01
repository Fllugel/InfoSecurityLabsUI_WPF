using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;
using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab4ViewModel : INotifyPropertyChanged
    {
        private string _selectedFileName;
        private string _selectDecryptFileName;
        private readonly RSA _rsa;

        public Lab4ViewModel()
        {
            _rsa = new RSA();
            GenerateKeysCommand = new RelayCommand(GenerateKeys);
            SelectFileCommand = new RelayCommand(SelectFile);
            EncryptCommand = new RelayCommand(EncryptFile);
            SelectDecryptFileCommand = new RelayCommand(SelectDecryptFile);
            DecryptCommand = new RelayCommand(DecryptFile);
            SavePublicKeyCommand = new RelayCommand(SavePublicKey);
            SavePrivateKeyCommand = new RelayCommand(SavePrivateKey);
            LoadPublicKeyCommand = new RelayCommand(LoadPublicKey);
            LoadPrivateKeyCommand = new RelayCommand(LoadPrivateKey);
        }

        public ICommand GenerateKeysCommand { get; }
        public ICommand SelectFileCommand { get; }
        public ICommand EncryptCommand { get; }
        public ICommand SelectDecryptFileCommand { get; }
        public ICommand DecryptCommand { get; }
        public ICommand SavePublicKeyCommand { get; }
        public ICommand SavePrivateKeyCommand { get; }
        public ICommand LoadPublicKeyCommand { get; }
        public ICommand LoadPrivateKeyCommand { get; }

        public string SelectedFileName
        {
            get => _selectedFileName;
            set
            {
                _selectedFileName = value;
                OnPropertyChanged();
            }
        }

        public string SelectDecryptFileName
        {
            get => _selectDecryptFileName;
            set
            {
                _selectDecryptFileName = value;
                OnPropertyChanged();
            }
        }

        public string PublicKey
        {
            get => _rsa.PublicKey;
            set
            {
                _rsa.PublicKey = value;
                OnPropertyChanged();
            }
        }

        public string PrivateKey
        {
            get => _rsa.PrivateKey;
            set
            {
                _rsa.PrivateKey = value;
                OnPropertyChanged();
            }
        }

        private void SavePublicKey()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = $"Text Files (*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, PublicKey);
            }
        }

        private void SavePrivateKey()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, PrivateKey);
            }
        }

        private void LoadPublicKey()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PublicKey = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void LoadPrivateKey()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PrivateKey = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void GenerateKeys()
        {
            _rsa.GenerateKeys();
            OnPropertyChanged(nameof(PublicKey));
            OnPropertyChanged(nameof(PrivateKey));
        }

        private async void EncryptFile()
        {
            if (string.IsNullOrWhiteSpace(SelectedFileName) || string.IsNullOrWhiteSpace(PublicKey))
            {
                MessageBox.Show("Please select a file and load the public key.");
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Encrypted Files (*.enc)|*.enc",
                    FileName = Path.GetFileName(SelectedFileName) + ".enc"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    await Task.Run(() => _rsa.EncryptFile(SelectedFileName, saveFileDialog.FileName));
                    stopwatch.Stop();
                    MessageBox.Show($"File encrypted successfully to: {saveFileDialog.FileName} in {stopwatch.ElapsedMilliseconds} ms.","Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during encryption: {ex.Message}");
            }
        }

        private async void DecryptFile()
        {
            if (string.IsNullOrWhiteSpace(SelectDecryptFileName) || string.IsNullOrWhiteSpace(PrivateKey))
            {
                MessageBox.Show("Please select an encrypted file and load the private key.");
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = Path.GetFileNameWithoutExtension(SelectDecryptFileName)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    await Task.Run(() => _rsa.DecryptFile(SelectDecryptFileName, saveFileDialog.FileName));
                    stopwatch.Stop();
                    MessageBox.Show($"File decrypted successfully to: {saveFileDialog.FileName} in {stopwatch.ElapsedMilliseconds} ms.","Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during decryption: {ex.Message}");
            }
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFileName = openFileDialog.FileName;
            }
        }

        private void SelectDecryptFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Encrypted Files (*.enc)|*.enc"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectDecryptFileName = openFileDialog.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}