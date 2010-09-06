//-----------------------------------------------------------------------
// <copyright file="FileSystem.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System.IO;

	public class FileSystem
	{
		public virtual bool DirectoryExists(string directoryToTest)
		{
			return Directory.Exists(directoryToTest);
		}

		public virtual void CreateDirectory(string directoryToCreate)
		{
			Directory.CreateDirectory(directoryToCreate);
		}
	}
}