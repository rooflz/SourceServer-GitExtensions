//-----------------------------------------------------------------------
// <copyright file="GitContentGetter.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;
	using System.Text.RegularExpressions;

	public class GitContentGetter
	{
		private const string InvalidRepository = "fatal: Not a git repository";
		private const string BadObjectId = "fatal: bad object";
		private static readonly Regex MissingObjectIdRegex =
			new Regex(@"^fatal\: ambiguous argument '.+'\: unknown revision or path not in the working tree\.");

		private readonly CommandRunner runner;
		private readonly Uri workingDirectory;

		public GitContentGetter(CommandRunner commandRunner, Uri workingDirectory)
		{
			this.runner = commandRunner;
			this.workingDirectory = workingDirectory;
		}

		public virtual string GetContents(string objectId)
		{
			var showCommand = GitCommandBuilder.CreateShowCommand(this.workingDirectory, objectId);
			var commandOutput = this.runner.GetOutputFrom(showCommand);
			ThrowOnInvalidRepository(commandOutput.ErrorOutput ?? string.Empty);
			ThrowOnMissingObjectId(commandOutput.ErrorOutput ?? string.Empty);
			return commandOutput.StandardOutput;
		}

		private static void ThrowOnInvalidRepository(string contents)
		{
			if (string.IsNullOrEmpty(contents))
				return;

			if (contents.StartsWith(InvalidRepository, StringComparison.Ordinal))
				throw new SourceProviderException(GitMessages.InvalidRepository);
		}

		private static void ThrowOnMissingObjectId(string contents)
		{
			if (contents.StartsWith(BadObjectId) || MissingObjectIdRegex.IsMatch(contents))
				throw new SourceProviderException(GitMessages.ObjectIdNotFound);
		}
	}
}