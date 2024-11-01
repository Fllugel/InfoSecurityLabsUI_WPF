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
                using (var aes = Aes.Create())
                {
                    rsa.FromXmlString(PublicKey);

                    // Encrypt the AES key and IV with RSA
                    byte[] encryptedAesKey = rsa.Encrypt(aes.Key, false);
                    byte[] encryptedAesIV = rsa.Encrypt(aes.IV, false);

                    // Encrypt file data with AES
                    byte[] fileData = File.ReadAllBytes(SelectedFileName);
                    byte[] encryptedData;
                    using (var ms = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(fileData, 0, fileData.Length);
                        cryptoStream.FlushFinalBlock();
                        encryptedData = ms.ToArray();
                    }

                    // Prompt user to save the encrypted file and AES key/IV
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Encrypted Files (*.enc)|*.enc",
                        FileName = Path.GetFileName(SelectedFileName) + ".enc"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        using (var bw = new BinaryWriter(fs))
                        {
                            // Write encrypted AES key, IV, and encrypted data
                            bw.Write(encryptedAesKey.Length);
                            bw.Write(encryptedAesKey);
                            bw.Write(encryptedAesIV.Length);
                            bw.Write(encryptedAesIV);
                            bw.Write(encryptedData.Length);
                            bw.Write(encryptedData);
                        }

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
                using (var aes = Aes.Create())
                {
                    rsa.FromXmlString(PrivateKey);

                    byte[] encryptedData;
                    using (var fs = new FileStream(SelectDecryptFileName, FileMode.Open))
                    using (var br = new BinaryReader(fs))
                    {
                        // Read encrypted AES key and IV
                        int aesKeyLength = br.ReadInt32();
                        byte[] encryptedAesKey = br.ReadBytes(aesKeyLength);
                        int aesIVLength = br.ReadInt32();
                        byte[] encryptedAesIV = br.ReadBytes(aesIVLength);

                        // Decrypt AES key and IV
                        aes.Key = rsa.Decrypt(encryptedAesKey, false);
                        aes.IV = rsa.Decrypt(encryptedAesIV, false);

                        // Read encrypted file data
                        int encryptedDataLength = br.ReadInt32();
                        encryptedData = br.ReadBytes(encryptedDataLength);
                    }

                    // Decrypt the file data with AES
                    byte[] decryptedData;
                    using (var ms = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                        cryptoStream.FlushFinalBlock();
                        decryptedData = ms.ToArray();
                    }

                    // Prompt user to save the decrypted file
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    { 
                        FileName = Path.GetFileNameWithoutExtension(SelectDecryptFileName)
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