using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Amber
{
    [ProtoContract]
    class Task : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _number, _state, _login;
        private int _length;
        private object _sync = new object();

        public Task(string number, int length, string state, string login)
        {
            _number = number;
            _length = length;
            _state = state;
            _login = login;
        }

        public Task()
        {
            
        }

        [ProtoMember(1)]
        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                NotifyPropertyChanged("Number");
            }
        }

        [ProtoMember(2)]
        public int Length
        {
            get { return _length; }
            set
            {
                _length = value;
                NotifyPropertyChanged("Length");
            }
        }

        [ProtoMember(3)]
        public string State
        {
            get
            {
                lock (_sync)
                {
                    return _state;
                }
            }
            set
            {
                lock (_sync)
                {
                    _state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        [ProtoMember(4)]
        public string Login
        {
            get
            {
                lock (_sync)
                {
                    return _login;
                }
            }
            set
            {
                lock (_sync)
                {
                    _login = value;
                    NotifyPropertyChanged("Login");
                }
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
