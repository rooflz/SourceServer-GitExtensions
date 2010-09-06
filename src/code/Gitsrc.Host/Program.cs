//-----------------------------------------------------------------------
// <copyright file="Program.cs">
//     Copyright (c) Jonathan Oliver, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gitsrc.Host
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class Program
	{
		private static void Main()
		{
			var arguments = new RuntimeArguments();

			var repositoryNodes = (from repositoryUrl in arguments.NodeUrls
								select OpenRepository(repositoryUrl, arguments.WorkingDirectory)).ToArray();

			var localNodes = repositoryNodes.Where(repo => repo.IsLocal);
			var remoteNodes = repositoryNodes.Where(repo => !repo.IsLocal);

			var contentsForObjectId = GetContents(localNodes, arguments.ObjectId)
				?? GetContents(remoteNodes, arguments.ObjectId)
				?? string.Empty;

			Console.Write(contentsForObjectId);
		}

		private static IRepository OpenRepository(string repositoryPath, Uri workingDirectory)
		{
			var repositoryFactory = new RepositoryFactory(localWorkingDirectory =>
				new Git(new CommandRunner(), new FileSystem(), localWorkingDirectory));

			return repositoryFactory.Create(repositoryPath, workingDirectory);
		}

		private static string GetContents(IEnumerable<IRepository> repositories, string objectId)
		{
			foreach (var repository in repositories)
			{
				try
				{
					return repository.GetContents(objectId);
				}
				catch (SourceProviderException)
				{
				}
			}

			return null;
		}
	}
}