using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Loader
{
    class RunningRelease
    {
        public RunningRelease(string path)
        {
            this.path = path;
            metadata = new string[METADATA_LENGTH];
        }

        private const int METADATA_LENGTH = 5; //Last line reads "EOF"

        private string path;
        private string[] metadata;
        private Process process;

        public string ReleaseName { get => metadata[0]; }
        public string ReleaseInfo { get => metadata[1]; }
        public string LaunchExec { get => metadata[2]; }
        public string LaunchArgs { get => metadata[3]; }
        public bool IsValid { get; private set; }

        public bool TryStart()
        {
            //Set default
            IsValid = false;
            Stop();
            
            //If the directory doesn't even exist, abort
            if (!Directory.Exists(path))
                return false;

            //Load metainfo file
            string metainfoFile = path + "release.metainfo";
            if (!File.Exists(metainfoFile))
                return false;
            string[] meta = File.ReadAllLines(metainfoFile);
            if (meta.Length != METADATA_LENGTH)
                return false;

            //Load props
            metadata = meta;

            //Start
            try
            {
                Launch();
            } catch (Exception)
            {
                //Likely linux and we need to update the permissions
                RunCLI("chmod 777 " + LaunchExec);
                Launch();
            }
            IsValid = true;
            return true;
        }

        private void Launch()
        {
            process = Process.Start(new ProcessStartInfo
            {
                Arguments = LaunchArgs,
                FileName = path + LaunchExec,
                UseShellExecute = false
            });
        }

        private void RunCLI(string cmd)
        {
            process = Process.Start(new ProcessStartInfo
            {
                Arguments = $"-c \"{cmd}\"",
                WorkingDirectory = path,
                FileName = "/bin/bash",
                UseShellExecute = false
            });
            process.WaitForExit();
        }

        public void Stop()
        {
            process?.Kill(true);
            process = null;
        }
    }
}
