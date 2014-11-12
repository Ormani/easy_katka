using System;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;

namespace Amber
{
    class CallHandler
    {
        //readonly Softphone _softphone;
        CallInfo _callInfo;
        readonly MediaConnector _connector;
        readonly PhoneCallAudioSender _mediaSender;

        public CallHandler(CallInfo callInfo)
        {
            //_softphone = softphone;
            _callInfo = callInfo;
            _mediaSender = new PhoneCallAudioSender();
            _connector = new MediaConnector();
        }

        public void Start(IPhoneLine phoneLine)
        {
            
            /*var call = Accounts.Softphone.CreateCall(phoneLine, _callInfo.PhoneNumber);
            call.CallStateChanged += OutgoingCallStateChanged;
            _mediaSender.AttachToCall(call);
            call.Start();*/
        }

        public event EventHandler Completed;

        void TextToSpeech(string text, IPhoneCall currenCall)
        {
            var textToSpeech = new TextToSpeech();

            _connector.Connect(textToSpeech, _mediaSender);
            textToSpeech.AddAndStartText(text);
            textToSpeech.Stopped += (sender, args) => textToSpeech_Stopped(currenCall);
        }

        static void textToSpeech_Stopped(object sender)
        {
            var a = (IPhoneCall)sender;
            a.HangUp();
        }

        void OutgoingCallStateChanged(object sender, CallStateChangedArgs e)
        {
            /*var a = (IPhoneCall)sender;
            foreach (var task in Form1.Tasks.Where(task => a.DialInfo.DialedString == task.Number))
            {
                task.State = e.State.ToString();
                task.Login = a.PhoneLine.SIPAccount.UserName;
                Form1.Tasks.ResetBindings();*/
            }

           /* if (e.State == CallState.Answered)
                TextToSpeech(_callInfo.Message, a);

            else if (e.State.IsCallEnded())
            {
                Accounts.Softphone.AddAvailablePhoneLine(((IPhoneCall)sender).PhoneLine);
                var handler = Completed;
                if (handler != null)
                    handler(_callInfo, EventArgs.Empty);*/
            }
        
}
