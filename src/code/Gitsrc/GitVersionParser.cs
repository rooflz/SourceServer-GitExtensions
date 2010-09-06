//-----------------------------------------------------------------------
// <copyright file="GitVersionParser.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System.Text.RegularExpressions;

	public class GitVersionParser
	{
		private static readonly Regex VersionRegex = new Regex(@"[0-9\.]+");

		private readonly CommandRunner runner;
		private readonly string version;

		public GitVersionParser(CommandRunner runner)
		{
			this.runner = runner;
			this.version = this.GetVersion();
			this.ThrowWhenVersionIsInvalid();
		}

		public string Version
		{
			get { return this.version; }
		}

		private string GetVersion()
		{
			var commandOutput = this.runner.GetOutputFrom(GitCommands.Version);
			return GetVersionFromOutput(commandOutput.StandardOutput);
		}

		private static string GetVersionFromOutput(string commandOutput)
		{
			return VersionRegex.Match(commandOutput ?? string.Empty).ToString();
		}

		private void ThrowWhenVersionIsInvalid()
		{
			if (string.IsNullOrEmpty(this.version))
				throw new SourceProviderException(GitMessages.GitNotFound);
		}
	}
}