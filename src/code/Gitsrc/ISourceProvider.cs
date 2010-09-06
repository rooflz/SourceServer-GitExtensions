//-----------------------------------------------------------------------
// <copyright file="ISourceProvider.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	public interface ISourceProvider
	{
		string Version { get; }

		string GetContents(string objectId);

		void Update();

		void Fetch(string sourceRepository);
	}
}