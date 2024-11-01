using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab3ViewModel : INotifyPropertyChanged
    {
        private string _selectedFileName;
        private string _selectDecryptFileName;

        private RC5? _rc5;
        private readonly MD5? _md5;
        private readonly ConfigLoader _configLoader;

        private const string ErrorMessage = "Error";

        public Lab3ViewModel()
        {
            _md5 = new MD5();

            _configLoader = new ConfigLoader();
            _configLoader.LoadConfigLab3();

            EncryptCommand = new RelayCommand(EncryptFile);
            DecryptCommand = new RelayCommand(DecryptFile);
            SelectFileCommand = new RelayCommand(SelectFile);
            SelectDecryptFileCommand = new RelayCommand(SelectDecryptFile);
        }

        public ICommand EncryptCommand { get; }
        public ICommand DecryptCommand { get; }
        public ICommand SelectFileCommand { get; }
        public ICommand SelectDecryptFileCommand { get; }

        private BitArray _passwordPhrase;
        public string PasswordPhrase
        {
            get => BitArrayToString(_passwordPhrase);
            set
            {
                _passwordPhrase = GetEncryptionKeyFromPassword(value, _configLoader.Lab3PasswordPhraseLength);
                byte[] passwordBytes = BitArrayToByteArray(_passwordPhrase);
                _rc5 = new RC5(passwordBytes, _configLoader.Lab3MD5Modulus, _configLoader.Lab3MD5Multiplier, _configLoader.Lab3MD5Increment, _configLoader.Lab3MD5Seed, _configLoader.Lab3RC5WordSize, _configLoader.Lab3RC5Rounds);
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

        private async void EncryptFile()
        {
            if (_rc5 == null)
            {
                MessageBox.Show("Encryption key is not set.", ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(_selectedFileName))
            {
                MessageBox.Show("No file selected for encryption.", ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] fileData = await File.ReadAllBytesAsync(_selectedFileName);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            byte[] encryptedFileData = await Task.Run(() => _rc5.Encrypt(fileData));
            stopwatch.Stop();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string outputFile = saveFileDialog.FileName;
                await File.WriteAllBytesAsync(outputFile, encryptedFileData);
                MessageBox.Show($"File encrypted successfully in {stopwatch.ElapsedMilliseconds} ms.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void DecryptFile()
        {
            if (_rc5 == null)
            {
                MessageBox.Show("Decryption key is not set.", ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(_selectDecryptFileName))
            {
                MessageBox.Show("No file selected for decryption.", ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] fileData = await File.ReadAllBytesAsync(_selectDecryptFileName);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            byte[] decryptedFileData = await Task.Run(() => _rc5.Decrypt(fileData));
            stopwatch.Stop();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string outputFile = saveFileDialog.FileName;
                await File.WriteAllBytesAsync(outputFile, decryptedFileData);
                MessageBox.Show($"File decrypted successfully in {stopwatch.ElapsedMilliseconds} ms.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFileName = openFileDialog.FileName;
            }
        }

        private void SelectDecryptFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectDecryptFileName = openFileDialog.FileName;
            }
        }

        private BitArray GetEncryptionKeyFromPassword(string password, int bitsCount)
        {
            if (_md5 == null)
            {
                throw new InvalidOperationException("MD5 instance is not initialized.");
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = _md5.ComputeHash(passwordBytes);

            ulong hashAsInt = BitConverter.ToUInt64(hashBytes, 0);

            BitArray encryptionKey = new BitArray(bitsCount);
            for (int i = 0; i < bitsCount; i++)
            {
                encryptionKey[i] = (hashAsInt & (1UL << (bitsCount - 1 - i))) != 0;
            }

            return encryptionKey;
        }

        private static byte[] BitArrayToByteArray(BitArray bitArray)
        {
            int numBytes = (bitArray.Length + 7) / 8;
            byte[] bytes = new byte[numBytes];
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }

        private static string BitArrayToString(BitArray bitArray)
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