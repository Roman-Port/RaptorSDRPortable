using RomanPort.LibEmbeddedSDR.Framework.Drawing.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR
{
    public class FontStore
    {
        public static readonly SdrFontPack SYSTEM_REGULAR_10;
        public static readonly SdrFontPack SYSTEM_REGULAR_15;
        public static readonly SdrFontPack SYSTEM_REGULAR_20;

        public static readonly SdrFontPack SYSTEM_BOLD_10;
        public static readonly SdrFontPack SYSTEM_BOLD_15;
        public static readonly SdrFontPack SYSTEM_BOLD_20;

        public static readonly SdrFontPack SYSTEM_COMPACT_10;
        public static readonly SdrFontPack SYSTEM_COMPACT_15;
        public static readonly SdrFontPack SYSTEM_COMPACT_20;

        static FontStore()
        {
            SYSTEM_REGULAR_10 = SdrFontPack.FromResource("SYSTEM_REGULAR_10");
            SYSTEM_REGULAR_15 = SdrFontPack.FromResource("SYSTEM_REGULAR_15");
            SYSTEM_REGULAR_20 = SdrFontPack.FromResource("SYSTEM_REGULAR_20");

            SYSTEM_BOLD_10 = SdrFontPack.FromResource("SYSTEM_BOLD_10");
            SYSTEM_BOLD_15 = SdrFontPack.FromResource("SYSTEM_BOLD_15");
            SYSTEM_BOLD_20 = SdrFontPack.FromResource("SYSTEM_BOLD_20");

            SYSTEM_COMPACT_10 = SYSTEM_REGULAR_10.MakeNarrowPack(0.75f);
            SYSTEM_COMPACT_15 = SYSTEM_REGULAR_15.MakeNarrowPack(0.75f);
            SYSTEM_COMPACT_20 = SYSTEM_REGULAR_20.MakeNarrowPack(0.75f);
        }
    }
}
