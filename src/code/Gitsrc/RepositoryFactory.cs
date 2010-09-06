//-----------------------------------------------------------------------
// <copyright file="RepositoryFactory.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;

	public class RepositoryFactory
	{
		private readonly Func<Uri, ISourceProvider> sourceProviderFactory;

		public RepositoryFactory(Func<Uri, ISourceProvider> sourceProviderFactory)
		{
			this.sourceProviderFactory = sourceProviderFactory;
		}

		public virtual IRepository Create(string pathToRepository, Uri workingDirectory)
		{
			EnsureValidPathToRepository(pathToRepository);

			if (IsLocalFileSystemPath(pathToRepository))
				return this.CreateLocalRepository(pathToRepository);

			EnsureLocalWorkingDirectoryForRemoteRepositories(workingDirectory);

			return this.CreateRemoteRepository(pathToRepository, workingDirectory);
		}

		private IRepository CreateLocalRepository(string pathToRepository)
		{
			var workingDirectory = new Uri(pathToRepository);
			var sourceProvider = this.sourceProviderFactory(workingDirectory);
			return new LocalRepository(sourceProvider);
		}
		private IRepository CreateRemoteRepository(string pathToRepository, Uri workingDirectory)
		{
			var sourceProvider = this.sourceProviderFactory(workingDirectory);
			return new RemoteRepository(sourceProvider, pathToRepository);
		}

		private static void EnsureValidPathToRepository(string pathToRepository)
		{
			if (string.IsNullOrEmpty(pathToRepository))
				throw new ArgumentNullException("pathToRepository");
		}
		private static void EnsureLocalWorkingDirectoryForRemoteRepositories(Uri workingDirectory)
		{
			if (null == workingDirectory)
				throw new ArgumentNullException("workingDirectory");

			if (!IsLocalFileSystemPath(workingDirectory))
				throw new ArgumentException("Working directory must be local.", "workingDirectory");
		}

		private static bool IsLocalFileSystemPath(string pathToRepository)
		{
			try
			{
				return IsLocalFileSystemPath(new Uri(pathToRepository));
			}
			catch (UriFormatException)
			{
				return false;
			}
		}
		private static bool IsLocalFileSystemPath(Uri location)
		{
			return location.IsFile;
		}
	}
}