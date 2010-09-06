//-----------------------------------------------------------------------
// <copyright file="LocalRepositoryFixture.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Tests
{
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class LocalRepositoryFixture
	{
		[Test]
		public virtual void GetContents_forwards_to_source_provider()
		{
			var objectId = "1234";
			var expectedContents = "test file contents";

			var sourceProviderStub = new Mock<ISourceProvider>();
			sourceProviderStub.Setup(x => x.GetContents(objectId)).Returns(expectedContents);

			var localRepository = new LocalRepository(sourceProviderStub.Object);
			var actualContents = localRepository.GetContents(objectId);

			Assert.AreEqual(expectedContents, actualContents);
		}

		[Test]
		public virtual void Single_retry_after_exception()
		{
			var objectId = "1234";

			var sourceProviderMock = new Mock<ISourceProvider>();
			sourceProviderMock.Setup(x => x.GetContents(objectId)).Throws<SourceProviderException>();

			sourceProviderMock.Setup(x => x.Update()).Verifiable();

			var localRepository = new LocalRepository(sourceProviderMock.Object);

			try
			{
				localRepository.GetContents(objectId);
			}
			catch (SourceProviderException)
			{
			}

			sourceProviderMock.VerifyAll();
		}
	}
}