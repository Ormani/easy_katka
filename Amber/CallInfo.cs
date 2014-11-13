﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ozeki.Media.MediaHandlers.Speech;

namespace Amber
{
    public struct CallInfo
    {
        public string PhoneNumber { get; private set; }
        public string Message { get; private set; }
        public string State { get; set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
        public VoiceInfo Voice { get; private set; }

        public CallInfo(string phoneNumber, string message, VoiceInfo voice, string state, int startTime, int endTime)
            : this()
        {
            PhoneNumber = phoneNumber;
            Message = message;
            Voice = voice;
            State = state;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
