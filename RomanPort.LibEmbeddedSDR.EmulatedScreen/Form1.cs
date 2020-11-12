using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.FFT;
using RomanPort.LibEmbeddedSDR.Framework.UI.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomanPort.LibEmbeddedSDR.EmulatedScreen
{
    public partial class Form1 : Form
    {
        private SdrSession session;
        private Thread worker;
        
        public Form1()
        {
            InitializeComponent();
            session = new SdrSession(emulatedDisplay1.display, 16384);
            worker = new Thread(session.Run);
            worker.IsBackground = true;
            worker.Start();
        }

        private void btnForceRedraw_Click(object sender, EventArgs e)
        {
            
        }

        private void OnInputControlBtnClicked(object sender, EventArgs e)
        {
            Enum.TryParse<UserInputKey>((string)((Button)sender).Tag, out UserInputKey key);
            session.OnUserInput(key);
        }

        private void btnScreencap_Click(object sender, EventArgs e)
        {
            //Copy
            Bitmap b = (Bitmap)emulatedDisplay1.GetBuffer().Clone();

            //Prompt
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Screenshot Output";
            dialog.Filter = "PNG Files (*.png)|*.png";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            //Write
            using(FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
                b.Save(fs, System.Drawing.Imaging.ImageFormat.Png);

            //Clean up
            b.Dispose();
        }
    }
}
