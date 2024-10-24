using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32; 

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab4ViewModel : INotifyPropertyChanged
    {
        private string _selectedFileName;
        private string _selectDecryptFileName;
        private string _publicKey;
        private string _privateKey;

        public Lab4ViewModel()
        {
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
            get => _publicKey;
            set
            {
                _publicKey = value;
                OnPropertyChanged();
            }
        }

        public string PrivateKey
        {
            get => _privateKey;
            set
            {
                _privateKey = value;
                OnPropertyChanged();
            }
        }

        private void SavePublicKey()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt"
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
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                PublicKey = rsa.ToXmlString(false); // Export public key
                PrivateKey = rsa.ToXmlString(true); // Export private key
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

        private void EncryptFile()
        {
            if (string.IsNullOrWhiteSpace(SelectedFileName) || string.IsNullOrWhiteSpace(PublicKey))
            {
                MessageBox.Show("Please select a file and load the public key.");
                return;
            }
        
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(PublicKey);
                    byte[] fileData = File.ReadAllBytes(SelectedFileName);
                    byte[] encryptedData = rsa.Encrypt(fileData, false);
        
                    // Prompt user to save the encrypted file
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Encrypted Files (*.enc)|*.enc",
                        FileName = Path.GetFileName(SelectedFileName) + ".enc" // Default name
                    };
        
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, encryptedData);
                        MessageBox.Show($"File encrypted successfully to: {saveFileDialog.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during encryption: {ex.Message}");
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

        private void DecryptFile()
        {
            if (string.IsNullOrWhiteSpace(SelectDecryptFileName) || string.IsNullOrWhiteSpace(PrivateKey))
            {
                MessageBox.Show("Please select an encrypted file and load the private key.");
                return;
            }
        
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(PrivateKey);
                    byte[] encryptedData = File.ReadAllBytes(SelectDecryptFileName);
                    byte[] decryptedData = rsa.Decrypt(encryptedData, false);
        
                    // Prompt user to save the decrypted file
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Decrypted Files (*.dec)|*.dec",
                        FileName = Path.GetFileNameWithoutExtension(SelectDecryptFileName) + ".dec" // Default name
                    };
        
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, decryptedData);
                        MessageBox.Show($"File decrypted successfully to: {saveFileDialog.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during decryption: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
