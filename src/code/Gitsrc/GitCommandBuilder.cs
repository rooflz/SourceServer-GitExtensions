//-----------------------------------------------------------------------
// <copyright file="GitCommandBuilder.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;
	using System.Globalization;
	using System.Text.RegularExpressions;

	internal static class GitCommandBuilder
	{
		private static readonly Regex ValidObjectId = new Regex("[0-9ABCDEF]", RegexOptions.IgnoreCase);

		public static string CreateShowCommand(Uri localRepositoryPath, string objectId)
		{
			ThrowOnInvalidObjectId(objectId);
			return CreateCommand(GitCommands.Show, localRepositoryPath.ConvertToLocalPathWhenIsLocal(), objectId);
		}

		public static string CreateFetchCommand(Uri targetPath, string sourcePath)
		{
			return CreateCommand(GitCommands.Fetch, targetPath.ConvertToLocalPathWhenIsLocal(), sourcePath);
		}

		public static string CreatePullCommand(Uri localRepositoryPath)
		{
			return CreateCommand(GitCommands.Pull, localRepositoryPath.ConvertToLocalPathWhenIsLocal());
		}

		private static string CreateCommand(string command, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, command, args);
		}

		private static void ThrowOnInvalidObjectId(string objectId)
		{
			if (string.IsNullOrEmpty(objectId) || !ValidObjectId.IsMatch(objectId))
				throw new ArgumentException(GitMessages.InvalidObjectId, "objectId");
		}

		private static string ConvertToLocalPathWhenIsLocal(this Uri repositoryLocation)
		{
			return repositoryLocation.IsFile ? repositoryLocation.LocalPath : repositoryLocation.ToString();
		}
	}
}