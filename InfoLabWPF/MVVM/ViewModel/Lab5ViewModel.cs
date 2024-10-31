﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab5ViewModel : INotifyPropertyChanged
    {
        private string _publicKey;
        private string _privateKey;
        private string _messageToSign;
        private string _messageSignature;
        private string _selectedFileNameToSign;
        private string _fileSignature;
        private string _selectedFileNameToVerify;
        private string _signatureToVerify;

        public Lab5ViewModel()
        {
            GenerateKeysCommand = new RelayCommand(GenerateKeys);
            SavePublicKeyCommand = new RelayCommand(SavePublicKey);
            LoadPublicKeyCommand = new RelayCommand(LoadPublicKey);
            SavePrivateKeyCommand = new RelayCommand(SavePrivateKey);
            LoadPrivateKeyCommand = new RelayCommand(LoadPrivateKey);
            SignMessageCommand = new RelayCommand(SignMessage);
            SelectFileToSignCommand = new RelayCommand(SelectFileToSign);
            SignFileCommand = new RelayCommand(SignFile);
            SelectFileToVerifyCommand = new RelayCommand(SelectFileToVerify);
            VerifySignatureCommand = new RelayCommand(VerifySignature);
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

        public string MessageToSign
        {
            get => _messageToSign;
            set
            {
                _messageToSign = value;
                OnPropertyChanged();
            }
        }

        public string MessageSignature
        {
            get => _messageSignature;
            set
            {
                _messageSignature = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFileNameToSign
        {
            get => _selectedFileNameToSign;
            set
            {
                _selectedFileNameToSign = value;
                OnPropertyChanged();
            }
        }

        public string FileSignature
        {
            get => _fileSignature;
            set
            {
                _fileSignature = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFileNameToVerify
        {
            get => _selectedFileNameToVerify;
            set
            {
                _selectedFileNameToVerify = value;
                OnPropertyChanged();
            }
        }

        public string SignatureToVerify
        {
            get => _signatureToVerify;
            set
            {
                _signatureToVerify = value;
                OnPropertyChanged();
            }
        }

        public ICommand GenerateKeysCommand { get; }
        public ICommand SavePublicKeyCommand { get; }
        public ICommand LoadPublicKeyCommand { get; }
        public ICommand SavePrivateKeyCommand { get; }
        public ICommand LoadPrivateKeyCommand { get; }
        public ICommand SignMessageCommand { get; }
        public ICommand SelectFileToSignCommand { get; }
        public ICommand SignFileCommand { get; }
        public ICommand SelectFileToVerifyCommand { get; }
        public ICommand VerifySignatureCommand { get; }

        private void GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048)) // Ensure key size is within the supported range
            {
                var publicKeyParams = rsa.ExportParameters(false);
                var privateKeyParams = rsa.ExportParameters(true);
                PublicKey = rsa.ToXmlString(false);
                PrivateKey = rsa.ToXmlString(true);
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

        private void SignMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageToSign) || string.IsNullOrWhiteSpace(PrivateKey))
            {
                MessageBox.Show("Please enter a message and load the private key.");
                return;
            }

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PrivateKey);
                byte[] messageBytes = Encoding.UTF8.GetBytes(MessageToSign);
                byte[] signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                MessageSignature = Convert.ToBase64String(signatureBytes);
            }
        }

        private void SelectFileToSign()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFileNameToSign = openFileDialog.FileName;
            }
        }

        private void SignFile()
        {
            if (string.IsNullOrWhiteSpace(SelectedFileNameToSign) || string.IsNullOrWhiteSpace(PrivateKey))
            {
                MessageBox.Show("Please select a file and load the private key.");
                return;
            }

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PrivateKey);
                byte[] fileData = File.ReadAllBytes(SelectedFileNameToSign);
                byte[] signatureBytes = rsa.SignData(fileData, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                FileSignature = Convert.ToBase64String(signatureBytes);
            }
        }

        private void SelectFileToVerify()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFileNameToVerify = openFileDialog.FileName;
            }
        }

        private void VerifySignature()
        {
            if (string.IsNullOrWhiteSpace(SelectedFileNameToVerify) ||
                string.IsNullOrWhiteSpace(PublicKey) ||
                string.IsNullOrWhiteSpace(SignatureToVerify))
            {
                MessageBox.Show("Please select a file, load the public key, and enter the signature.");
                return;
            }

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PublicKey);
                byte[] fileData = File.ReadAllBytes(SelectedFileNameToVerify);
                byte[] signatureBytes;
                try
                {
                    signatureBytes = Convert.FromBase64String(SignatureToVerify);
                }
                catch (FormatException)
                {
                    MessageBox.Show("The provided signature is not a valid Base64 string.");
                    return;
                }
                bool isValid = rsa.VerifyData(fileData, signatureBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                MessageBox.Show(isValid ? "Signature is valid." : "Signature is invalid.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}