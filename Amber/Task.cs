using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Amber
{
    class Task : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _number, _state, _login, _time;
        private int _length;

        public Task(string number, int length, string state, string login, string time)
        {
            _number = number;
            _length = length;
            _state = state;
            _login = login;
            _time = time;
        }

        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                NotifyPropertyChanged("Number");
            }
        }

        public int Length
        {
            get { return _length; }
            set
            {
                _length = value;
                NotifyPropertyChanged("Length");
            }
        }

        public string State
        {
            get
            {
                lock (_state)
                {
                    return _state;
                }
            }
            set
            {
                lock (_state)
                {
                    _state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        public string Login
        {
            get
            {
                lock (_login)
                {
                    return _login;
                }
            }
            set
            {
                lock (_login)
                {
                    _login = value;
                    NotifyPropertyChanged("Login");
                }
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                NotifyPropertyChanged("Time");
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
