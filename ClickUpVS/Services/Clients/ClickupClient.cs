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
		public Task<TaskDetail> GetTaskAsync([Path] string taskId, [Query("include_subtasks", Name = "include_subtasks")] string includeSubtasks, CancellationToken cancellationToken = default);

		[Get("task/{taskId}/comment")]
		public Task<GetTaskComments> GetTaskCommentsAsync([Path] string taskId, CancellationToken cancellationToken = default);

		[Post("task/{taskId}/comment")]
		public Task<CreateTaskCommentResponse> CreateTaskCommentAsync([Path] string taskId, [Body] CreateTaskCommentRequest request, CancellationToken cancellationToken = default);

		[Get("user")]
		public Task<AuthorizedUser> GetAuthorizedUserAsync(CancellationToken cancellationToken = default);

		[Delete("comment/{commentId}")]
		public Task DeleteCommentAsync([Path] string commentId, CancellationToken cancellationToken = default);

		[Put("checklist/{checklistId}/checklist_item/{checklistItemId}")]
		public Task UpdateChecklistAsync([Path] string checklistId, [Path] string checklistItemId, [Body] UpdateChecklistItemRequest request, CancellationToken cancellationToken = default);

		[Post("checklist/{checklistId}/checklist_item")]
		public Task<CreateChecklistItemResponse> CreateChecklistItemAsync([Path] string checklistId, [Body] CreateChecklistItemRequest request, CancellationToken cancellationToken = default);

		[Post("list/{listId}/task")]
		public Task<TaskDetail> CreateTaskAsync([Path] string listId, [Body] CreateTaskRequest request, CancellationToken cancellationToken = default);

		[Put("task/{taskId}")]
		public Task UpdateTaskAsync([Path] string taskId, [Body] UpdateTaskRequest request, CancellationToken cancellationToken = default);

		[Get("space/{spaceId}")]
		public Task<Space> GetSpaceAsync([Path] string spaceId, CancellationToken cancellationToken = default);

		[Get("list/{listId}")]
		public Task<List> GetListAsync([Path] string listId, CancellationToken cancellationToken = default);
	}
}
