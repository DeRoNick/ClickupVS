using ClickUpVS.Models;
using ClickUpVS.Services.Clients;
using RestEase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUpVS.Services
{
	internal class ClickupService
	{
		private readonly IClickupClient _client;

		public ClickupService(string apiKey)
		{
			_client = RestClient.For<IClickupClient>("https://api.clickup.com/");
			_client.Auth = apiKey;
		}

		public async Task<List<Workspace>> GetWorkSpacesAsync(CancellationToken cancellationToken = default)
		{
			return (await _client.GetWorkspacesAsync(cancellationToken)).Teams;
		}

		public async Task<List<Space>> GetSpacesAsync(string workspaceId, CancellationToken cancellationToken = default)
		{
			var spaces = (await _client.GetSpacesAsync(workspaceId, cancellationToken)).Spaces;

			foreach (var space in spaces)
			{
				var folders = (await _client.GetFoldersAsync(space.Id, cancellationToken)).Folders;
				space.Folders = folders;

				var lists = (await _client.GetFolderlessListsAsync(space.Id, cancellationToken)).Lists;
				space.Lists = lists;
			}

			return spaces;
		}
	}
}
