//-----------------------------------------------------------------------
// <copyright file="GitRepositoryFetcher.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;
	using System.Text.RegularExpressions;

	public class GitRepositoryFetcher
	{
		private static readonly Regex NoRepositoryFound = new Regex(@"fatal\: .+?/info/refs not found\:");

		private readonly CommandRunner runner;
		private readonly FileSystem filesystem;
		private readonly Uri workingDirectory;

		public GitRepositoryFetcher(CommandRunner runner, FileSystem fileSystem, Uri workingDirectory)
		{
			this.runner = runner;
			this.filesystem = fileSystem;
			this.workingDirectory = workingDirectory;
		}

		public virtual void Fetch(string sourceRepository)
		{
			this.filesystem.CreateDirectory(this.workingDirectory.LocalPath);

			var fetchCommand = GitCommandBuilder.CreateFetchCommand(this.workingDirectory, sourceRepository);
			var commandOutput = this.runner.GetOutputFrom(fetchCommand);

			ThrowOnInvalidRepository(commandOutput.ErrorOutput);
		}

		private static void ThrowOnInvalidRepository(string command)
		{
			if (NoRepositoryFound.IsMatch(command))
				throw new SourceProviderException(GitMessages.RepositoryNotFound);
		}
	}
}