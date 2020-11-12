using RomanPort.LibEmbeddedSDR.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.LibEmbeddedSDR.EmulatedScreen
{
    public unsafe class EmulatedDisplaySession : ISdrDisplay
    {
        public EmulatedDisplaySession(int width, int height, EmulatedDisplay view) : base(width, height)
        {
            this.view = view;
        }

        private EmulatedDisplay view;

        public override void Apply()
        {
            view.UpdateBuffer(bufferPtr);
        }
    }
}
