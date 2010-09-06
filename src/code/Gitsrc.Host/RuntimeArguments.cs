//-----------------------------------------------------------------------
// <copyright file="RuntimeArguments.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Host
{
	using System;
	using System.Collections.Generic;
	using NDesk.Options;

	public class RuntimeArguments
	{
		private const string NodeDelimiter = " ";

		private string repositorySources = string.Empty;

		public RuntimeArguments()
		{
			var p = new OptionSet
			{
				{ "nodes=", value => this.repositorySources = value ?? string.Empty },
				{ "working=", value => this.WorkingDirectory = ParseWorkingDirectory(value) },
				{ "id=", value => this.ObjectId = value }
			};

			p.Parse(Environment.GetCommandLineArgs());
		}

		public IEnumerable<string> NodeUrls
		{
			get { return Split(this.repositorySources); }
		}

		public string ObjectId { get; private set; }

		public Uri WorkingDirectory { get; private set; }

		private static Uri ParseWorkingDirectory(string workingDirectory)
		{
			Uri parsed;
			Uri.TryCreate(workingDirectory, UriKind.Absolute, out parsed);
			return parsed;
		}

		private static IEnumerable<string> Split(string value)
		{
			value += NodeDelimiter;

			return value.Split(NodeDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		}
	}
}