using System.Diagnostics;
using UnityEngine;

namespace BaseFramework
{
    public class CommandUtil
    {
        public class CommandResult
        {
            public int resultCode;
            public string output;
            public string error;

            public CommandResult(int resultCode, string output, string error)
            {
                this.resultCode = resultCode;
                this.output = output;
                this.error = error;
            }
        }

        public static CommandResult ExecuteCommand(string command)
        {
            command = command.Replace("\"", "\\\"");

            ProcessStartInfo processInfo = null;

            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    processInfo = new ProcessStartInfo("/bin/bash", $"-c \"{command}\"");
                    break;
                case RuntimePlatform.WindowsEditor:
                    processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                    break;
            }

            if (processInfo == null)
            {
                return null;
            }
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            Process process = Process.Start(processInfo);
            if (process != null)
            {
                process.WaitForExit();

                CommandResult commandResult = new CommandResult(process.ExitCode,
                                                                process.StandardOutput.ReadToEnd(),
                                                                process.StandardError.ReadToEnd());

                process.Close();

                return commandResult;
            }
            return new CommandResult(-1, "execute error", "process is null");
        }
    }
}
