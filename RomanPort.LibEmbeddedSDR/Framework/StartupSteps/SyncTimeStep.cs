using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.StartupSteps
{
    public class SyncTimeStep : IStartupStep
    {
        private bool hasTimeUpdated;
        private bool cancel;
        private SdrSession session;
        
        public string GetErrorTitle()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Syncing time... (B to cancel)";
        }

        public void OnUserInput(UserInputKey key)
        {
            cancel = cancel || key == UserInputKey.B;
        }

        public bool Work(SdrSession session, out string errorText)
        {
            this.session = session;
            session.radio.demodulatorWbFm.UseRds().OnTimeUpdated += SyncTimeStep_OnTimeUpdated;
            session.radio.source.CenterFrequency = 97100000;
            session.radio.source.AutoGainEnabled = true;
            while(!hasTimeUpdated && !cancel)
            {
                session.radio.ProcessBlock();
            }
            session.radio.source.CenterFrequency = 92500000;
            session.radio.source.ManualGainLevel = 6;
            session.radio.demodulatorWbFm.UseRds().OnTimeUpdated -= SyncTimeStep_OnTimeUpdated;
            errorText = "";
            return true;
        }

        private void SyncTimeStep_OnTimeUpdated(LibSDR.Extras.RDS.RDSClient client, DateTime time, TimeSpan offset)
        {
            hasTimeUpdated = true;
            session.SetTime(time);
        }
    }
}
