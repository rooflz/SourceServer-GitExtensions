//-----------------------------------------------------------------------
// <copyright file="CommandOutput.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	public class CommandOutput
	{
		public string StandardOutput { get; set; }

		public string ErrorOutput { get; set; }

		public int ExitCode { get; set; }
	}
}