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

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab3ViewModel : INotifyPropertyChanged
{
    private BitArray _passwordPhrase;
    private string _selectedFileName;
    private string _selectDecryptFileName;
    private byte[] _encryptedFileData;
    private byte[] _decryptedFileData;

    private RC5? _rc5;
    private readonly MD5? _md5;
    private readonly ConfigLoader _configLoader;

    public Lab3ViewModel()
    {
        _md5 = new MD5();
        _configLoader = new ConfigLoader();
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

    private void EncryptFile()
    {
        if (_rc5 == null)
        {
            MessageBox.Show("Encryption key is not set.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrEmpty(_selectedFileName))
        {
            MessageBox.Show("No file selected for encryption.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        byte[] fileData = File.ReadAllBytes(_selectedFileName);
        _encryptedFileData = _rc5.Encrypt(fileData);

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        if (saveFileDialog.ShowDialog() == true)
        {
            string outputFile = saveFileDialog.FileName;
            File.WriteAllBytes(outputFile, _encryptedFileData);
            MessageBox.Show("File encrypted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DecryptFile()
    {
        if (_rc5 == null)
        {
            MessageBox.Show("Decryption key is not set.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrEmpty(_selectDecryptFileName))
        {
            MessageBox.Show("No file selected for decryption.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        byte[] fileData = File.ReadAllBytes(_selectDecryptFileName);
        _decryptedFileData = _rc5.Decrypt(fileData);

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        if (saveFileDialog.ShowDialog() == true)
        {
            string outputFile = saveFileDialog.FileName;
            File.WriteAllBytes(outputFile, _decryptedFileData);
            MessageBox.Show("File decrypted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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