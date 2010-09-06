//-----------------------------------------------------------------------
// <copyright file="RemoteRepository.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	public class RemoteRepository : IRepository
	{
		private readonly ISourceProvider provider;
		private readonly string remoteRepositoryLocation;

		public RemoteRepository(ISourceProvider provider, string remoteRepositoryLocation)
		{
			this.provider = provider;
			this.remoteRepositoryLocation = remoteRepositoryLocation;
		}

		public bool IsLocal
		{
			get { return false; }
		}

		public string GetContents(string objectId)
		{
			this.provider.Fetch(this.remoteRepositoryLocation);
			return this.provider.GetContents(objectId);
		}
	}
}