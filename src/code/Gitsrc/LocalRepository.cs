//-----------------------------------------------------------------------
// <copyright file="LocalRepository.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	public class LocalRepository : IRepository
	{
		private readonly ISourceProvider provider;

		public LocalRepository(ISourceProvider provider)
		{
			this.provider = provider;
		}

		public bool IsLocal
		{
			get { return true; }
		}

		public string GetContents(string objectId)
		{
			try
			{
				return this.provider.GetContents(objectId);
			}
			catch (SourceProviderException)
			{
				return this.RetryGetContentsOnce(objectId);
			}
		}

		private string RetryGetContentsOnce(string objectId)
		{
			this.provider.Update();
			return this.provider.GetContents(objectId);
		}
	}
}