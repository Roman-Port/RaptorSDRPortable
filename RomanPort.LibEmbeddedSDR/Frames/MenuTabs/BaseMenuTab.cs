using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Frames.MenuTabs
{
    public abstract class BaseMenuTab : BaseRenderComponent
    {
        public BaseMenuTab(IDrawingContext parent) : base(parent)
        {
        }

        public abstract string DisplayName { get; }
    }
}
