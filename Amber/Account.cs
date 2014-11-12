using System.ComponentModel;
using ProtoBuf;

namespace Amber
{
    [ProtoContract]
    class Account : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _login, _password, _server, _state;

        public Account(string login, string password, string server, string state)
        {
            _login = login;
            _password = password;
            _server = server;
            _state = state;
        }

        [ProtoMember(1)]
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                NotifyPropertyChanged("Login");
            }
        }

        [ProtoMember(2)]
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
            }
        }

        [ProtoMember(3)]
        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                NotifyPropertyChanged("Server");
            }
        }

        [ProtoMember(4)]
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyPropertyChanged("State");
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
