using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace InfoLabWPF.MVVM.ViewModel
{
    public class Lab5ViewModel : INotifyPropertyChanged
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string MessageToSign { get; set; }
        public string MessageSignature { get; set; }
        public string SelectedFileNameToSign { get; set; }
        public string FileSignature { get; set; }
        public string SelectedFileNameToVerify { get; set; }
        public string SignatureToVerify { get; set; }

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

        private void GenerateKeys() { /* Implementation */ }
        private void SavePublicKey() { /* Implementation */ }
        private void LoadPublicKey() { /* Implementation */ }
        private void SavePrivateKey() { /* Implementation */ }
        private void LoadPrivateKey() { /* Implementation */ }
        private void SignMessage() { /* Implementation */ }
        private void SelectFileToSign() { /* Implementation */ }
        private void SignFile() { /* Implementation */ }
        private void SelectFileToVerify() { /* Implementation */ }
        private void VerifySignature() { /* Implementation */ }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}