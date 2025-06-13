using ClickUpVS.Models;
using ClickUpVS.Services.Clients;
using ClickUpVS.Services.Clients.Models;
using Newtonsoft.Json.Serialization;
using RestEase;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUpVS.Services
{
	internal class ClickupService
	{
		private readonly IClickupClient _client;
		private readonly User CurrentUser;

		public ClickupService(string apiKey)
		{
			_client = new RestClient("https://api.clickup.com/")
			{
				JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
				{
					ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new SnakeCaseNamingStrategy()
					}
				}
			}.For<IClickupClient>();
			_client.Auth = apiKey;
			CurrentUser = _client.GetAuthorizedUserAsync().Result.User;
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

		public async Task<List<List>> GetListWithTasksAsync(string folderId, CancellationToken cancellationToken = default)
		{
			var lists = await GetListsAsync(folderId, cancellationToken);

			foreach (var list in lists)
			{
				var tasks = await GetTasksAsync(list.Id, cancellationToken);

				list.Tasks = tasks;
			}

			return lists;
		}

		public async Task<List<List>> GetListsAsync(string folderId, CancellationToken cancellationToken = default)
		{
			return (await _client.GetListsAsync(folderId, cancellationToken)).Lists;
		}

		public async Task<List<TaskItem>> GetTasksAsync(string listId, CancellationToken cancellationToken = default)
		{
			return (await _client.GetTasksAsync(listId, cancellationToken)).Tasks;
		}

		public async Task<TaskDetail> GetTaskAsync(string taskId, CancellationToken cancellationToken = default)
		{
			var task = await _client.GetTaskAsync(taskId, includeSubtasks: "true", cancellationToken); // their api doesnt accept True as a boolean, and im too lazy to write a converter

			task.Checklists = [.. task.Checklists.Select(x =>
			{
				x.Items = [.. x.Items.Select(y => {
					y.ChecklistId = x.Id;
					return y;
				})];
				return x;
			})];

			task.Comments = [.. (await _client.GetTaskCommentsAsync(taskId, cancellationToken)).Comments.Select(x => {
				x.Deletable = x.User.Id == CurrentUser.Id;
				return x;
			}).OrderBy(x => x.Date)];

			return task;
		}

		public async Task<Comment> CreateTaskCommentAsync(string taskId, string comment, CancellationToken cancellationToken = default)
		{
			var result = await _client.CreateTaskCommentAsync(taskId, new() { CommentText = comment }, cancellationToken);

			return new Comment()
			{
				CommentText = comment,
				Date = result.Date,
				Deletable = true,
				Id = result.Id,
				Reactions = [],
				User = CurrentUser
			};
		}

		public async Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default)
		{
			await _client.DeleteCommentAsync(commentId, cancellationToken);
		}

		public async Task UpdateChecklistItemAsync(string checklistId, string checklistItemId, ChecklistItem item, CancellationToken cancellationToken = default)
		{
			await _client.UpdateChecklistAsync(checklistId, checklistItemId, new()
			{
				Name = item.Name,
				Resolved = item.Resolved
			}, cancellationToken);
		}

		public async Task<ChecklistItem> CreateChecklistItemAsync(string checklistId, string itemName, CancellationToken cancellationToken = default)
		{
			var result = await _client.CreateChecklistItemAsync(checklistId, new() { Name = itemName }, cancellationToken);

			var item = result.Checklist.Items.First(x => x.Name == itemName);

			item.Resolved = false;
			item.ChecklistId = checklistId;

			return item;
		}

		public async Task<TaskDetail> CreateTaskAsync(string listId, CreateTaskRequest request, CancellationToken cancellationToken = default)
		{
			return await _client.CreateTaskAsync(listId, request, cancellationToken);
		}

		public async Task UpdateTaskStatusAsync(string taskId, string status, CancellationToken cancellationToken = default)
		{
			await _client.UpdateTaskAsync(taskId, new()
			{
				Status = status
			});
		}

		public async Task<List<Models.TaskStatus>> GetAvailableStatusesAsync(string spaceId, CancellationToken cancellationToken = default)
		{
			var result = await _client.GetSpaceAsync(spaceId, cancellationToken);

			return result.Statuses;
		}

	}
}
