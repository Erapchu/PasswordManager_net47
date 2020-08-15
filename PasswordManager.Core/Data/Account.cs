using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{
    [DebuggerDisplay("User password = {CorrectPassword}, Count of credentials = {Credentials.Count}")]
    [Serializable]
    public class Account : INotifyPropertyChanged
    {
        [JsonProperty]
        public CredentialsCollection Credentials { get; set; } = new CredentialsCollection();

        [JsonProperty]
        public SortType CredentialsSort { get; set; }

        [JsonProperty]
        public string CorrectPassword { get; private set; }

        public bool SetNewPassword(string newPassword)
        {
            CorrectPassword = newPassword;
            return Configuration.Instance.SaveAccount("New password has been initialized");
        }

        #region MVVM Pattern
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
