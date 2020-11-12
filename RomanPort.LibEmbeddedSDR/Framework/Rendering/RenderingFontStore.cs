using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    public class RenderingFontStore
    {
        private Dictionary<RenderingFontTag, SdrFontPack> fonts;

        public RenderingFontStore()
        {
            this.fonts = new Dictionary<RenderingFontTag, SdrFontPack>();
        }

        public BaseFontPack GetFontPack(RenderingFontTag tag)
        {
            if (fonts.ContainsKey(tag))
                return fonts[tag];
            else
                return new NullFontPack();
        }

        public void LoadFontPackFromPath(RenderingFontTag tag, string path)
        {
            var pack = SdrFontPack.LoadFromBytes(File.ReadAllBytes(path));
            fonts.Add(tag, pack);
        }

        public void LoadFontPackFromResource(RenderingFontTag tag, string name)
        {
            byte[] data = SdrSession.UtilGetAssetBytes(name);
            var pack = SdrFontPack.LoadFromBytes(data);
            fonts.Add(tag, pack);
        }
    }
}
