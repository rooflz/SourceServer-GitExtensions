//-----------------------------------------------------------------------
// <copyright file="IRepository.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	public interface IRepository
	{
		bool IsLocal { get; }

		string GetContents(string objectId);
	}
}