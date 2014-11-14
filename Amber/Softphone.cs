
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ozeki.VoIP;
using Ozeki.VoIP.SDK;


namespace Amber
{
    class Softphone
    {
        readonly ISoftPhone _softphone;
        IPhoneLine _phoneLine;
        SIPAccount _account;
        readonly List<SIPAccount> _registeredSipAccounts = new List<SIPAccount>();
        readonly List<IPhoneLine> _availablePhoneLines = new List<IPhoneLine>();
        public event EventHandler<RegistrationStateChangedArgs> PhoneLineStateChanged;

        public Softphone()
        {
            _softphone = SoftPhoneFactory.CreateSoftPhone(5000, 10000);
        }

        public void Register(bool registrationRequired, string displayName, string userName, string authenticationId, string registerPassword, string domainHost, int domainPort)
        {
            try
            {
                _account = new SIPAccount(registrationRequired, displayName, userName, authenticationId, registerPassword, domainHost, domainPort);
                _phoneLine = _softphone.CreatePhoneLine(_account);
                _phoneLine.RegistrationStateChanged += _phoneLine_RegistrationStateChanged;
                _softphone.RegisterPhoneLine(_phoneLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during SIP registration: {0}", ex.Message);
            }
        }

        void _phoneLine_RegistrationStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            if (e.State == RegState.RegistrationSucceeded)
            {
                _registeredSipAccounts.Add(_account);
                _availablePhoneLines.Add(_phoneLine);
            }
            var handler = PhoneLineStateChanged;
            if (handler != null)
                handler(_account, e);
        }

        public int GetAvailablePhoneLineCount()
        {
            lock (_availablePhoneLines)
                return _availablePhoneLines.Count;
        }

        public IPhoneLine GetAvailablePhoneLine()
        {
            var randomLine = new Random();
            var number = randomLine.Next(GetAvailablePhoneLineCount());
            lock (_availablePhoneLines)
                return _availablePhoneLines[number];
        }

        public IPhoneLine GetAvailablePhoneLine(string userName, string domainHost)
        {
            lock (_availablePhoneLines)
            {
                foreach (var phoneLine in _availablePhoneLines.Where(phoneLine =>
                    phoneLine.SIPAccount.UserName.Equals(userName) &&
                    phoneLine.SIPAccount.DomainServerHost.Equals(domainHost)))
                {
                    return phoneLine;
                }
                Console.WriteLine("No available phone line with those attributes! First available phone line is being selected.");
                return _availablePhoneLines[0];
            }
        }

        public void AddAvailablePhoneLine(IPhoneLine phoneLine)
        {
            lock (_availablePhoneLines)
                _availablePhoneLines.Add(phoneLine);
        }

        public void RemoveAvailablePhoneLine(IPhoneLine phoneLine)
        {
            lock (_availablePhoneLines)
                _availablePhoneLines.Remove(phoneLine);
        }

        public void UnregAllPhoneLines()
        {
            lock (_availablePhoneLines)
                foreach (var availablePhoneLine in _availablePhoneLines)
                {
                    _softphone.UnregisterPhoneLine(availablePhoneLine);
                }
        }

        public IPhoneCall CreateCall(IPhoneLine phoneLine, string member)
        {
            return _softphone.CreateCallObject(phoneLine, member);
        }

    }
}
