//-----------------------------------------------------------------------
// <copyright file="GitFixture.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Tests
{
	using System;
	using System.Globalization;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class GitFixture
	{
		private static readonly Uri DefaultWorkingDirectory = new Uri("file://c:/temp.git");
		private Mock<CommandRunner> commandRunnerFake;
		private Mock<FileSystem> fileSystemFake;
		private Git git;

		[SetUp]
		public virtual void Setup()
		{
			this.commandRunnerFake = new Mock<CommandRunner>();
			this.fileSystemFake = new Mock<FileSystem>();
			this.AddVersionCommandToCommandRunner();
			this.git = new Git(this.commandRunnerFake.Object, this.fileSystemFake.Object, DefaultWorkingDirectory);
		}

		[Test]
		public virtual void Throw_when_null_CommandRunner()
		{
			CommandRunner invalid = null;
			var valid = DefaultWorkingDirectory;
			Assert.Throws<ArgumentNullException>(() => new Git(invalid, this.fileSystemFake.Object, valid));
		}

		[Test]
		public virtual void Throw_when_working_directory_does_not_exist()
		{
			var valid = this.commandRunnerFake.Object;
			Uri invalid = null;
			Assert.Throws<ArgumentNullException>(() => new Git(valid, this.fileSystemFake.Object, invalid));
		}

		[Test]
		public virtual void Throw_when_git_executable_binary_not_in_path()
		{
			this.AddCommandToCommandRunner(GitCommands.Version, string.Empty, string.Empty);
			Assert.Throws<SourceProviderException>(() => 
				new Git(this.commandRunnerFake.Object, this.fileSystemFake.Object, DefaultWorkingDirectory));
		}

		[Test]
		public virtual void Version_properly_parsed()
		{
			Assert.AreEqual(GitCommandsOutput.ExpectedVersion, this.git.Version);
		}

		[Test]
		public virtual void Throw_when_remote_working_directory()
		{
			var remote = new Uri("http://github.com/altnetlibs/gitsrc.git");
			Assert.Throws<SourceProviderException>(() => 
				new Git(this.commandRunnerFake.Object, this.fileSystemFake.Object, remote));
		}

		[Test]
		public virtual void Hexadecimal_objectId_only()
		{
			var nonHexObjectId = "HIJKLMNOP";

			Assert.Throws<ArgumentException>(() =>
				GitCommandBuilder.CreateShowCommand(DefaultWorkingDirectory, nonHexObjectId));
		}

		[Test]
		public virtual void Command_to_show_contents_built_from_parameters_provided()
		{
			var expectedCommand = "git --git-dir=\"" + DefaultWorkingDirectory.LocalPath + "\" --no-pager show MyObjectId";
			var actualCommand = GitCommandBuilder.CreateShowCommand(DefaultWorkingDirectory, "MyObjectId");
			Assert.AreEqual(expectedCommand, actualCommand);
		}

		[Test]
		public virtual void GetContents_returns_file_content()
		{
			var objectId = "12345";

			var showCommand = GitCommandBuilder.CreateShowCommand(DefaultWorkingDirectory, objectId);
			this.AddCommandToCommandRunner(showCommand, GitCommandsOutput.RawShowOutput, string.Empty);
			var output = this.git.GetContents(objectId);

			Assert.AreEqual(GitCommandsOutput.RawShowOutput, output);
		}

		[Test]
		public virtual void GetContents_throws_on_missing_repository()
		{
			var objectId = "1234";
			var showCommand = GitCommandBuilder.CreateShowCommand(DefaultWorkingDirectory, objectId);
			this.AddCommandToCommandRunner(showCommand, string.Empty, GitCommandsOutput.MissingRepository);

			Assert.Throws<SourceProviderException>(() => this.git.GetContents(objectId));
		}

		[Test]
		public virtual void GetContents_throws_on_nonexistent_objectId()
		{
			var nonexistentObjectId = "12345";

			var showCommand = GitCommandBuilder.CreateShowCommand(DefaultWorkingDirectory, nonexistentObjectId);
			this.AddCommandToCommandRunner(showCommand, string.Empty, GitCommandsOutput.InvalidObjectIdShowOutput);

			Assert.Throws<SourceProviderException>(() => this.git.GetContents(nonexistentObjectId));
		}

		[Test]
		public virtual void Command_to_fetch_built_from_parameters_provided()
		{
			var source = @"C:\MySource";
			var target = new Uri("file://C:/MyTarget");

			var expectedCommand = "git --git-dir=\"C:\\MyTarget\" init && git --git-dir=\"C:\\MyTarget\" fetch \"C:\\MySource\"";
			var actualCommand = GitCommandBuilder.CreateFetchCommand(target, source);
			Assert.AreEqual(expectedCommand, actualCommand);
		}

		[Test]
		public virtual void Fetch_creates_working_directory()
		{
			var sourceRepository = "git://github.com/altnetlibs/gitsrc.git";
			this.AddFetchCommandToCommandRunner(sourceRepository, string.Empty, string.Empty);

			this.fileSystemFake.Setup(x => x.CreateDirectory(DefaultWorkingDirectory.LocalPath)).Verifiable();
			this.git.Fetch(sourceRepository);

			this.fileSystemFake.VerifyAll();
		}

		[Test]
		public virtual void Fetch_executes_git_fetch_command()
		{
			var sourceRepository = "git://github.com/altnetlibs/gitsrc.git";

			var fetchCommand = GitCommandBuilder.CreateFetchCommand(DefaultWorkingDirectory, sourceRepository);
			this.AddCommandToCommandRunner(fetchCommand, string.Empty, string.Empty);

			this.git.Fetch(sourceRepository);

			this.commandRunnerFake.VerifyAll();
		}

		[Test]
		public virtual void Fetch_throws_on_invalid_source_repository()
		{
			var nonexistentRepository = "http://www.google.com/.git";

			var fetchOutput = FormatInvalidSourceRepoOutput(nonexistentRepository);
			this.AddFetchCommandToCommandRunner(nonexistentRepository, string.Empty, fetchOutput);

			Assert.Throws<SourceProviderException>(() => this.git.Fetch(nonexistentRepository));
		}

		[Test]
		public virtual void Update_executes_git_pull_command()
		{
			var pullCommand = GitCommandBuilder.CreatePullCommand(DefaultWorkingDirectory);
			this.AddCommandToCommandRunner(pullCommand, string.Empty, string.Empty);
			this.git.Update();

			this.commandRunnerFake.VerifyAll();
		}

		private void AddVersionCommandToCommandRunner()
		{
			this.AddCommandToCommandRunner(GitCommands.Version, GitCommandsOutput.RawVersionOutput, string.Empty);
		}

		private void AddFetchCommandToCommandRunner(string sourceRepository, string output, string error)
		{
			var fetchCommand = GitCommandBuilder.CreateFetchCommand(DefaultWorkingDirectory, sourceRepository);
			this.AddCommandToCommandRunner(fetchCommand, output, error);
		}

		private void AddCommandToCommandRunner(string command, string result, string error)
		{
			var output = new CommandOutput
			{
				StandardOutput = result,
				ErrorOutput = error
			};

			this.commandRunnerFake.Setup(x => x.GetOutputFrom(command)).Returns(output).Verifiable();
		}

		private static string FormatInvalidSourceRepoOutput(string nonexistentRepo)
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				GitCommandsOutput.FetchNoRepositoryFound,
				DefaultWorkingDirectory.LocalPath,
				nonexistentRepo);
		}
	}
}