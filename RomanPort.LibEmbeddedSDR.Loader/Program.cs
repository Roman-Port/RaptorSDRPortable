using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace RomanPort.LibEmbeddedSDR.Loader
{
    class Program
    {
        public const int VERSION_ID = 1;
        public const int PORT = 43167;

        public static string rootPath;
        public static string releasePath;

        public static RunningRelease release;

        static void Main(string[] args)
        {
            //Print info
            Console.WriteLine($"RaptorSDR Loader {VERSION_ID}");

            //Get the path
            rootPath = args[0] + Path.DirectorySeparatorChar;
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            releasePath = rootPath + "release" + Path.DirectorySeparatorChar;

            //Try to start
            release = new RunningRelease(releasePath);
            if (!release.TryStart())
                Console.WriteLine("Release is missing or invalid! You MUST upload a new one!");
            else
                Console.WriteLine($"Running release {release.ReleaseName}");

            //Launch server
            RunServer();
        }

        static void RunServer()
        {
            //Start
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + PORT + "/");
            listener.Start();

            //Loop
            while(true)
            {
                //Wait for a request
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                //Switch on the path
                int responseCode;
                string responseText;
                try
                {
                    switch (request.Url.AbsolutePath)
                    {
                        case "/":
                            //About endpoint
                            responseText = $"RaptorSDR Loader version {VERSION_ID}\n{(release.IsValid ? $"Running release {release.ReleaseName}" : "NO VALID RELEASE UPLOADED")}\n\n/ : About\n/upload_update : Accepts a ZIP file to apply as an upgrade.";
                            responseCode = 200;
                            break;
                        case "/upload_update":
                            //Applies an upgrade
                            responseCode = 200;
                            responseText = UploadUpgrade(request.InputStream);
                            break;
                        default:
                            responseCode = 404;
                            responseText = "Not Found";
                            break;
                    }
                } catch (HttpUserErrorException ex)
                {
                    responseCode = 400;
                    responseText = ex.Message;
                } catch (Exception ex)
                {
                    responseCode = 500;
                    responseText = $"There was an error: {ex.Message}{ex.StackTrace}";
                }

                //Write response
                HttpListenerResponse response = context.Response;
                byte[] content = Encoding.UTF8.GetBytes(responseText);
                response.StatusCode = responseCode;
                response.ContentType = "text/plain";
                response.ContentLength64 = content.Length;
                response.OutputStream.Write(content, 0, content.Length);
            }
        }

        static string UploadUpgrade(Stream stream)
        {
            //Read entirely
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;

            //Open ZIP file
            ZipArchive zip;
            try
            {
                zip = new ZipArchive(ms, ZipArchiveMode.Read);
            } catch
            {
                throw new HttpUserErrorException("A POST request with a valid ZIP file is expected. This ZIP file is either missing or invalid.");
            }

            //Ensure the metafile exists
            if (zip.GetEntry("release.metainfo") == null)
                throw new HttpUserErrorException("The uploaded ZIP file did not contain a release.metainfo file. This file is required.");

            //Stop current release
            release.Stop();
            Thread.Sleep(500);

            //Delete release folder
            if (Directory.Exists(releasePath))
                Directory.Delete(releasePath, true);

            //Extract ZIP to it
            zip.ExtractToDirectory(releasePath);

            //Start release
            if (release.TryStart())
                return $"OK; Now running release {release.ReleaseName}.";
            else
                throw new HttpUserErrorException("The uploaded release is invalid and failed to start. Try again.");
        }
    }
}
