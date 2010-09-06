//-----------------------------------------------------------------------
// <copyright file="SourceProviderException.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class SourceProviderException : Exception
	{
		public SourceProviderException()
		{
		}

		public SourceProviderException(string message)
			: base(message)
		{
		}

		public SourceProviderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SourceProviderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}