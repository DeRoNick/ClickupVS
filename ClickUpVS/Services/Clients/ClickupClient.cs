using ClickUpVS.Services.Clients.Models;
using RestEase;
using System.Threading.Tasks;
using System.Threading;
using ClickUpVS.Models;

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

		[Get("list/{listId}/task")]
		public Task<GetTasksResponse> GetTasksAsync([Path] string listId, CancellationToken cancellationToken = default);

		[Get("folder/{folderId}/list")]
		public Task<GetListsResponse> GetListsAsync([Path] string folderId, CancellationToken cancellationToken = default);

		[Get("task/{taskId}")]
		public Task<TaskDetail> GetTaskAsync([Path] string taskId, CancellationToken cancellationToken = default);

		[Get("task/{taskId}/comment")]
		public Task<GetTaskComments> GetTaskCommentsAsync([Path] string taskId, CancellationToken cancellationToken = default);

		[Post("task/{taskId}/comment")]
		public Task<CreateTaskCommentResponse> CreateTaskCommentAsync([Path] string taskId, [Body] CreateTaskCommentRequest request, CancellationToken cancellationToken = default);

		[Get("user")]
		public Task<AuthorizedUser> GetAuthorizedUserAsync(CancellationToken cancellationToken = default);
	}
}
