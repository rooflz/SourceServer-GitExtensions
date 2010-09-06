//-----------------------------------------------------------------------
// <copyright file="RepositoryFactoryFixture.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Tests
{
	using System;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RepositoryFactoryFixture
	{
		private readonly RepositoryFactory factory = 
			new RepositoryFactory(repositoryPath => new Mock<ISourceProvider>().Object);

		[Test]
		public virtual void Null_repository_path_throws()
		{
			string invalidRepositoryPath = null;
			Assert.Throws<ArgumentNullException>(() => this.factory.Create(invalidRepositoryPath, null));
		}

		[Test]
		public virtual void Local_uri_creates_LocalRepository()
		{
			var localPath = @"C:\";
			Uri workingDirectory = null;

			var repository = this.factory.Create(localPath, workingDirectory);

			Assert.That(repository is LocalRepository);
		}

		[Test]
		public virtual void Throw_when_missing_working_directory_for_remote_repositories()
		{
			var remotePath = "git://github.com/altnetlibs/gitsrc.git";
			Uri invalidWorkingDirectory = null;

			Assert.Throws<ArgumentNullException>(() => this.factory.Create(remotePath, invalidWorkingDirectory));
		}

		[Test]
		public virtual void Throw_when_remote_working_directory_for_remote_repositories()
		{
			var remotePath = "git://github.com/altnetlibs/gitsrc.git";
			var remoteWorkingDirectory = new Uri(remotePath);

			Assert.Throws<ArgumentException>(() => this.factory.Create(remotePath, remoteWorkingDirectory));
		}

		[Test]
		public virtual void Remote_uri_creates_RemoteRepository()
		{
			var remotePath = "git://github.com/altnetlibs/gitsrc.git";
			var workingDirectory = new Uri("file://C:/");

			var repository = this.factory.Create(remotePath, workingDirectory);

			Assert.That(repository is RemoteRepository);
		}
	}
}