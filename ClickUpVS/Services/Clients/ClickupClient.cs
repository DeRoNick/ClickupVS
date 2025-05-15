using ClickUpVS.Services.Clients.Models;
using RestEase;
using System.Threading.Tasks;
using System.Threading;

namespace ClickUpVS.Services.Clients
{
	[BasePath("api/v2")]
	[Header("accept", "application/json")]
	internal interface IClickupClient
	{
		[Header("Authorization")]
		public string Auth { get; set; }

		[Get("team")]
		public Task<GetWorkspacesResponse> GetWorkspacesAsync(CancellationToken cancellationToken = default);

		[Get("team/{workspaceId}/space")]
		public Task<GetSpacesResponse> GetSpacesAsync([Path] string workspaceId, CancellationToken cancellationToken = default);

		[Get("space/{spaceId}/folder")]
		public Task<GetFolders> GetFoldersAsync([Path] string spaceId, CancellationToken cancellationToken = default);

		[Get("space/{spaceId}/list")]
		public Task<GetFolderlessListsResponse> GetFolderlessListsAsync([Path] string spaceId, CancellationToken cancellationToken = default);
	}
}
