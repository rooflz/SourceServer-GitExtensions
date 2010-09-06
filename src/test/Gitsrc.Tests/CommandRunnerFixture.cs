//-----------------------------------------------------------------------
// <copyright file="CommandRunnerFixture.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class CommandRunnerFixture
	{
		[Test]
		public virtual void Ensure_standard_output_captured_from_command()
		{
			var runner = new CommandRunner();
			var result = runner.GetOutputFrom("echo Hello, World!");
			Assert.AreEqual("Hello, World!", result.StandardOutput);
		}
	}
}