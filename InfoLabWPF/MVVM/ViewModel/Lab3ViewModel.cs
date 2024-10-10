using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Collections;
using CommunityToolkit.Mvvm.Input;
using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab3ViewModel : INotifyPropertyChanged
    {
        private BitArray _passwordPhrase; 
        private string _selectedFileName;
        private string _selectDecryptFileName;
        private double _encryptionProgress;
        private double _decryptProgress;

        private RC5 _rc5;
        private MD5 _md5;

        public Lab3ViewModel()
        {
            _md5 = new MD5();
            EncryptCommand = new RelayCommand(EncryptFile);
            DecryptCommand = new RelayCommand(DecryptFile);
            SelectFileCommand = new RelayCommand(SelectFile);
            SelectDecryptFileCommand = new RelayCommand(SelectDecryptFile);
        }

        public ICommand EncryptCommand { get; }
        public ICommand DecryptCommand { get; }
        public ICommand SelectFileCommand { get; }
        public ICommand SelectDecryptFileCommand { get; }

        public string PasswordPhrase 
        {
            set
            {
                _passwordPhrase = GetEncryptionKeyFromPassword(value, 16);

                byte[] passwordBytes = BitArrayToByteArray(_passwordPhrase);
                string passwordString = Encoding.UTF8.GetString(passwordBytes);

                Console.WriteLine(BitArrayToString(_passwordPhrase)); 
                _rc5 = new RC5(passwordString); 
                OnPropertyChanged();
            }
        }

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

        public double EncryptionProgress
        {
            get => _encryptionProgress;
            set
            {
                _encryptionProgress = value;
                OnPropertyChanged();
            }
        }

        public double DecryptProgress
        {
            get => _decryptProgress;
            set
            {
                _decryptProgress = value;
                OnPropertyChanged();
            }
        }

        private void EncryptFile()
        {
            // Implement file encryption logic
        }

        private void DecryptFile()
        {
            // Implement file decryption logic
        }

        private void SelectFile()
        {
            // Implement file selection for encryption
        }

        private void SelectDecryptFile()
        {
            // Implement file selection for decryption
        }

        private BitArray GetEncryptionKeyFromPassword(string password, int bitsCount)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = _md5.ComputeHash(passwordBytes);

            ulong hashAsInt = BitConverter.ToUInt64(hashBytes, 0);

            // Convert hashAsInt to a bit array of the desired length (bitsCount)
            BitArray encryptionKey = new BitArray(bitsCount);
            for (int i = 0; i < bitsCount; i++)
            {
                encryptionKey[i] = (hashAsInt & (1UL << (bitsCount - 1 - i))) != 0;
            }

            return encryptionKey;
        }

        private byte[] BitArrayToByteArray(BitArray bitArray)
        {
            int numBytes = (bitArray.Length + 7) / 8; 
            byte[] bytes = new byte[numBytes];
            bitArray.CopyTo(bytes, 0); 
            return bytes;
        }

        private string BitArrayToString(BitArray bitArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (bool bit in bitArray)
            {
                sb.Append(bit ? "1" : "0");
            }
            return sb.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}