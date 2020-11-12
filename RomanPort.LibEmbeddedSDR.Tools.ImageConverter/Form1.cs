using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomanPort.LibEmbeddedSDR.Tools.ImageConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            Bitmap src = (Bitmap)Image.FromFile("E:\\input.png");
            FileStream output = new FileStream("E:\\output.sdrimg", FileMode.Create);
            Convert(src, output);
            output.Flush();
            output.Close();
        }

        private void Convert(Bitmap src, Stream dst)
        {
            //Prepare header
            //4     Sign    Always "SDIM" (SDR Image)
            //1     Byte    Version. Always 1
            //1     Byte    Flags
            //2     UShort  Width
            //2     UShort  Height
            byte[] header = new byte[10];
            header[0] = (byte)'S';
            header[1] = (byte)'D';
            header[2] = (byte)'I';
            header[3] = (byte)'M';
            header[4] = 0x01;
            header[5] = 0x01;
            BitConverter.GetBytes((ushort)src.Width).CopyTo(header, 6);
            BitConverter.GetBytes((ushort)src.Height).CopyTo(header, 8);
            dst.Write(header, 0, header.Length);

            //Now, read all pixels as integers
            int[] img = new int[src.Width * src.Height];
            for (int i = 0; i < src.Width * src.Height; i++)
            {
                img[i] = src.GetPixel(i % src.Width, i / src.Width).ToArgb();
            }

            //Now, we write each of the four channels in the following order: ARGB
            for(int i = 0; i<4; i++)
            {
                //Create space to store length
                long startIndex = dst.Position;
                dst.WriteByte(0x00);
                dst.WriteByte(0x00);
                dst.WriteByte(0x00);
                dst.WriteByte(0x00);

                //Open GZIP stream
                using (GZipStream gz = new GZipStream(dst, CompressionMode.Compress, true))
                    ConvertChannelPayload(img, gz, 3 - i);

                //Save end location
                long endIndex = dst.Position;

                //Rewind and write length
                dst.Position = startIndex;
                dst.Write(BitConverter.GetBytes((int)(endIndex - startIndex - 4)), 0, 4);

                //Jump back to end to begin next region
                dst.Position = endIndex;
            }
        }

        private void ConvertChannelPayload(int[] src, Stream dst, int channel)
        {
            for(int i = 0; i<src.Length; i++)
            {
                byte pixel = (byte)((src[i] >> (channel * 8)) & 0xFF);
                dst.WriteByte(pixel);
            }
        }
    }
}
