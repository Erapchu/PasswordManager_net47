using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{
    [Serializable]
    public class CredentialsCollection : ObservableCollection<Credentials>
    {
        public CredentialsCollection(IEnumerable<Credentials> credentials) 
        {
            foreach (var cred in credentials)
                this.Add(cred);
        }

        public CredentialsCollection() { }

        public Credentials this[Guid guid]
        {
            get
            {
                var currentCredential = this.FirstOrDefault(cred => cred.ID.Equals(guid));
                return currentCredential;
            }
            set
            {
                var currentCredential = this.FirstOrDefault(cred => cred.ID.Equals(guid));
                var currentCredentialIndex = this.IndexOf(currentCredential);
                this[currentCredentialIndex] = value;
            }
        }
    }
}
