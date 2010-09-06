//-----------------------------------------------------------------------
// <copyright file="CommandRunner.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;
	using System.Diagnostics;
	using System.IO;

	public class CommandRunner
	{
		private const string WindowsLineBreak = "\r\n";

		private const string RunWindowsCommandLine = "cmd.exe";

		private const string ArgumentPrefix = "/c ";

		public virtual CommandOutput GetOutputFrom(string arguments)
        {
			using (var process = CreateProcess(arguments))
			{
				process.Start();
				return CapturedOutputFrom(process);
			}
        }

		private static Process CreateProcess(string processArguments)
        {
            return new Process
            {
                StartInfo = DeclareProcessStartInfo(processArguments)
            };
        }

        private static ProcessStartInfo DeclareProcessStartInfo(string arguments)
        {
            return new ProcessStartInfo
            {
                FileName = RunWindowsCommandLine,
				Arguments = ArgumentPrefix + arguments,
                RedirectStandardOutput = true,
				RedirectStandardError = true,
                UseShellExecute = false
            };
        }

		private static CommandOutput CapturedOutputFrom(Process process)
		{
			return new CommandOutput
			{
				StandardOutput = CaptureOutputFromStream(process.StandardOutput),
				ErrorOutput = CaptureOutputFromStream(process.StandardError),
				ExitCode = process.ExitCode
			};
		}

		private static string CaptureOutputFromStream(TextReader reader)
		{
			var captured = reader.ReadToEnd() ?? string.Empty;
			return RemoveLastLineBreakFromOutput(captured);
		}

		private static string RemoveLastLineBreakFromOutput(string captured)
		{
			return EndsWithLineBreak(captured)
				? captured.Substring(0, captured.Length - WindowsLineBreak.Length)
				: captured;
		}

		private static bool EndsWithLineBreak(string value)
		{
			return !string.IsNullOrEmpty(value) && value.EndsWith(WindowsLineBreak, StringComparison.Ordinal);
		}
	}
}