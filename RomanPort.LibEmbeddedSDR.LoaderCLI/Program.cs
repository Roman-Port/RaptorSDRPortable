using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.LoaderCLI
{
    /* Just a quick and dirty CLI for communicating with the Pi
     * 
     * {IP Address}
     * {Command}
     * {Args}
     * 
     */
    class Program
    {
        public static string serverAddr;
        public static string cmd;
        public static string[] cmdArgs;

        static int Main(string[] args)
        {
            //Validate
            if(args.Length < 2)
            {
                Console.WriteLine("RaptorSDR CLI - Invalid usage: {IP Address} {Command} {Args}");
                return -1;
            }

            //Extract
            serverAddr = args[0];
            cmd = args[1];
            cmdArgs = new string[args.Length - 2];
            Array.Copy(args, 2, cmdArgs, 0, args.Length - 2);

            //Handle
            switch(cmd)
            {
                case "upgrade":
                    return RequestUpgrade();
                default:
                    Console.WriteLine("Unknown command. Commands are: upgrade");
                    return -2;
            }
        }

        static int RequestUpgrade()
        {
            //Validate and extract
            if(cmdArgs.Length != 3)
            {
                Console.WriteLine("Invalid command usage. Expected {release name} {release dir} {release exec}");
                return -3;
            }
            string releaseName = cmdArgs[0];
            string releaseDir = cmdArgs[1];
            string releaseExec = cmdArgs[2];

            //Build into ZIP
            MemoryStream ms = new MemoryStream();
            ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true);
            RequestUpgradeAddToZip(zip, releaseDir, releaseDir.Length);

            //Add metainfo
            byte[] metainfoData = Encoding.UTF8.GetBytes($"{releaseName}\n\n{releaseExec}\n\nEOF");
            using (var s = zip.CreateEntry("release.metainfo").Open())
                s.Write(metainfoData, 0, metainfoData.Length);

            //Close and rewind
            zip.Dispose();
            ms.Position = 0;

            //Send
            bool ok = 200 == UtilSendHttpPost("/upload_update", ms, out string info);
            Console.WriteLine(info);
            return ok ? 0 : -4;
        }

        static void RequestUpgradeAddToZip(ZipArchive zip, string dir, int substr)
        {
            string[] files = Directory.GetFiles(dir);
            string[] dirs = Directory.GetDirectories(dir);
            foreach(var f in files)
            {
                var e = zip.CreateEntry(dir.Substring(substr) + new FileInfo(f).Name);
                using (FileStream inp = new FileStream(f, FileMode.Open, FileAccess.Read))
                using (Stream oup = e.Open())
                    inp.CopyTo(oup);
            }
            foreach (var d in dirs)
                RequestUpgradeAddToZip(zip, d + Path.DirectorySeparatorChar, substr);
        }

        static int UtilSendHttpPost(string path, Stream body, out string text)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://" + serverAddr + ":43167" + path);
            request.Method = "POST";
            using (var stream = request.GetRequestStream())
                body.CopyTo(stream);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            } catch (WebException wex)
            {
                if(wex.Response == null)
                {
                    text = "Could not connect to the server.";
                    return -1;
                }
                response = (HttpWebResponse)wex.Response;
            }
            text = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return (int)response.StatusCode;
        }
    }
}
