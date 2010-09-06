//-----------------------------------------------------------------------
// <copyright file="RemoteRepositoryFixture.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Tests
{
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RemoteRepositoryFixture
	{
		[Test]
		public virtual void GetContents_fetches_from_remote_repository()
		{
			var remoteRepositoryUrl = "url of remote repository";

			var sourceProviderMock = new Mock<ISourceProvider>();
			sourceProviderMock.Setup(x => x.Fetch(remoteRepositoryUrl)).Verifiable();

			var remoteRepository = new RemoteRepository(sourceProviderMock.Object, remoteRepositoryUrl);

			remoteRepository.GetContents(string.Empty);

			sourceProviderMock.VerifyAll();
		}

		[Test]
		public virtual void GetContents_forwards_to_source_provider()
		{
			var objectId = "1234";
			var expectedContents = "My contents";

			var sourceProviderStub = new Mock<ISourceProvider>();
			sourceProviderStub.Setup(x => x.GetContents(objectId)).Returns(expectedContents);

			var remoteRepository = new RemoteRepository(sourceProviderStub.Object, string.Empty);

			var actualContents = remoteRepository.GetContents(objectId);

			Assert.AreEqual(expectedContents, actualContents);
		}
	}
}