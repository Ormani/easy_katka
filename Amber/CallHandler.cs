using System;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;

namespace Amber
{
    internal class CallHandler
    {
        public event EventHandler CallStateChanged;
        private readonly Softphone _softphone;
        private CallInfo _callInfo;
        private readonly MediaConnector _connector = new MediaConnector();
        private readonly PhoneCallAudioSender _mediaSender = new PhoneCallAudioSender();

        public CallHandler(CallInfo callInfo, Softphone softphone)
        {
            _softphone = softphone;
            _callInfo = callInfo;
        }

        public void Start(IPhoneLine phoneLine)
        {
            var call = _softphone.CreateCall(phoneLine, _callInfo.PhoneNumber);
            call.CallStateChanged += OutgoingCallStateChanged;
            _mediaSender.AttachToCall(call);
            call.Start();
        }

        private void TextToSpeech(string text, IBaseCall currenCall)
        {
            var textToSpeech = new TextToSpeech();
            _connector.Connect(textToSpeech, _mediaSender);
            textToSpeech.AddAndStartText(text);
            textToSpeech.Stopped += (sender, args) => textToSpeech_Stopped(currenCall);
        }

        private static void textToSpeech_Stopped(object sender)
        {
            var a = (IPhoneCall) sender;
            a.HangUp();
        }

        private void OutgoingCallStateChanged(object sender, CallStateChangedArgs e)
        {
            var a = (IPhoneCall) sender;
            
            if (e.State == CallState.Answered)
                TextToSpeech(_callInfo.Message, a);

            else if (e.State.IsCallEnded())
            {
                _softphone.UpdatePhoneLine(((IPhoneCall) sender).PhoneLine);
                TasksForm.CallListPublic.Add(_callInfo);
            }

            var handler = CallStateChanged;
            if (handler != null)
                handler(a, e);
        }
    }
}
