//-----------------------------------------------------------------------
// <copyright file="Git.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;

	public class Git : ISourceProvider
	{
		private readonly Uri workingDirectory;
		private readonly CommandRunner runner;
		private readonly GitVersionParser versionParser;
		private readonly GitContentGetter contentShower;
		private readonly GitRepositoryFetcher repositoryFetcher;

		public Git(CommandRunner runner, FileSystem fileSystem, Uri workingDirectory)
		{
			ThrowOnNullArguments(runner, workingDirectory);
			ThrowOnNonLocalRepository(workingDirectory);

			this.workingDirectory = workingDirectory;
			this.runner = runner;
			this.versionParser = new GitVersionParser(runner);
			this.contentShower = new GitContentGetter(runner, workingDirectory);
			this.repositoryFetcher = new GitRepositoryFetcher(runner, fileSystem, workingDirectory);
		}

		public string Version
		{
			get { return this.versionParser.Version; }
		}

		public void Update()
		{
			var pullCommand = GitCommandBuilder.CreatePullCommand(this.workingDirectory);
			this.runner.GetOutputFrom(pullCommand);
		}

		public void Fetch(string sourceRepository)
		{
			this.repositoryFetcher.Fetch(sourceRepository);
		}

		public string GetContents(string objectId)
		{
			return this.contentShower.GetContents(objectId);
		}

		private static void ThrowOnNullArguments(CommandRunner runner, Uri localRepositoryPath)
		{
			if (null == runner)
				throw new ArgumentNullException("runner");

			if (null == localRepositoryPath)
				throw new ArgumentNullException("localRepositoryPath");
		}

		private static void ThrowOnNonLocalRepository(Uri localRepositoryPath)
		{
			if (!localRepositoryPath.IsFile)
				throw new SourceProviderException(GitMessages.TargetRepositoryMustBeLocal);
		}
	}
}