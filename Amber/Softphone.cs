using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ozeki.VoIP;
using Ozeki.VoIP.SDK;


namespace Amber
{
    public class Softphone
    {
        readonly ISoftPhone _softphone;
        IPhoneLine _phoneLine;
        SIPAccount _account;
        readonly List<SIPAccount> _registeredSipAccounts = new List<SIPAccount>();
        //readonly List<IPhoneLine> _availablePhoneLines = new List<IPhoneLine>();
        readonly ConcurrentDictionary<IPhoneLine, bool> _phoneLinesDictionary = new ConcurrentDictionary<IPhoneLine, bool>(); 
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
                _phoneLinesDictionary.TryAdd(_phoneLine, true);
                //_availablePhoneLines.Add(_phoneLine);
            }

            var handler = PhoneLineStateChanged;
            if (handler != null)
                handler(_account, e);
        }

        public IPhoneLine GetAvailablePhoneLine()
        {
            return _phoneLinesDictionary.Where(phoneLine => 
                _phoneLinesDictionary.TryUpdate(phoneLine.Key, false, true)).Select(phoneLine => 
                    phoneLine.Key).FirstOrDefault();
        }

        /*public IPhoneLine GetAvailablePhoneLine(string userName, string domainHost)
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
        }*/

        public void UpdatePhoneLine(IPhoneLine phoneLine)
        {
            _phoneLinesDictionary.TryUpdate(phoneLine, true, false);
        }

        public void UnregPhoneLine(string userName)
        {
            foreach (var phoneLine in _phoneLinesDictionary.Where(phoneLine => phoneLine.Key.SIPAccount.UserName == userName))
            {
                bool temp;
                _account = phoneLine.Key.SIPAccount;
                _phoneLine = phoneLine.Key;
                _phoneLinesDictionary.TryRemove(_phoneLine, out temp);
                _registeredSipAccounts.Remove(_account);
                _softphone.UnregisterPhoneLine(_phoneLine);
                return;
            }
        }

        public void UnregAllPhoneLines()
        {
            foreach (var phoneLine in _phoneLinesDictionary)
            {
                _phoneLine = phoneLine.Key;
                _account = phoneLine.Key.SIPAccount;
                bool temp;
                _phoneLinesDictionary.TryRemove(_phoneLine, out temp);
                _registeredSipAccounts.Remove(_account);
                _softphone.UnregisterPhoneLine(phoneLine.Key);
            }
        }

        public IPhoneCall CreateCall(IPhoneLine phoneLine, string member)
        {
            return _softphone.CreateCallObject(phoneLine, member);
        }

    }
}
