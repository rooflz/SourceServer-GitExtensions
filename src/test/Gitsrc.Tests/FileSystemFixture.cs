//-----------------------------------------------------------------------
// <copyright file="FileSystemFixture.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;

	[TestFixture]
	public class FileSystemFixture
	{
		private readonly FileSystem filesystem = new FileSystem();
		private string randomDirectoryName;

		[SetUp]
		public void Setup()
		{
			this.randomDirectoryName = Guid.NewGuid().ToString();
		}

		[TearDown]
		public void Cleanup()
		{
			try
			{
				Directory.Delete(this.randomDirectoryName, true);
			}
			catch (DirectoryNotFoundException)
			{
			}
		}

		[Test]
		public void Ensure_invalid_directory_cannot_exist()
		{
			Assert.IsFalse(this.filesystem.DirectoryExists(this.randomDirectoryName));
		}

		[Test]
		public void Ensure_valid_directory_exists()
		{
			Assert.IsTrue(this.filesystem.DirectoryExists("C:/WINDOWS/"));
		}

		[Test]
		public void Ensures_directory_is_created()
		{
			Assert.IsFalse(Directory.Exists(this.randomDirectoryName));
			this.filesystem.CreateDirectory(this.randomDirectoryName);
			Assert.IsTrue(Directory.Exists(this.randomDirectoryName));
		}
	}
}