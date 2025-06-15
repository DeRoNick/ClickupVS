using ClickUpVS.Models;
using ClickUpVS.Services;
using ClickUpVS.Views.Models;
using Microsoft.VisualStudio.Threading;
using RestEase;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickUpVS
{
	public partial class ClickUpWindowControl : UserControl
	{
		private General _options;
		private ClickupService _service;
		private Stack<TaskDetail> _taskDetails = new();

		public ClickUpWindowControl()
		{
			InitializeComponent();
			_ = InitializeAsync();
			WorkspaceSelector.SelectionChanged += OnWorkspaceSelectionChanged;
			SpaceList.SelectionChanged += OnSpaceSelectionChanged;
			ProjectsList.ButtonClicked += OnTaskButtonClicked;
			ProjectsList.TaskDetailView.OnSendComment += OnSendComment;
			ProjectsList.TaskDetailView.OnDeleteComment += OnDeleteComment;
			ProjectsList.TaskDetailView.OnCheckChanged += OnCheckChanged;
			ProjectsList.TaskDetailView.OnAddTaskItem += OnAddTaskItem;
			ProjectsList.TaskDetailView.OnAddTask += OnAddTask;
			ProjectsList.TaskDetailView.OnStatusChanged += OnStatusChanged;
			ProjectsList.TaskDetailView.OnSubtaskButtonClicked += OnSubtaskButtonClicked;
			ProjectsList.BackButtonClicked += OnBackButtonClicked;
			ProjectsList.TaskDetailView.OnSaveDescriptionClicked += OnSaveDescriptionClicked;
			ProjectsList.TaskDetailView.OnSaveNameClicked += OnSaveNameClicked;
		}

		private async Task InitializeAsync()
		{
			_options = await General.GetLiveInstanceAsync();

			if (string.IsNullOrEmpty(_options.ApiKey))
			{
				ApiKeyPromptPanel.Visibility = Visibility.Visible;
				MainUIPanel.Visibility = Visibility.Collapsed;
			}
			else
			{
				ApiKeyPromptPanel.Visibility = Visibility.Collapsed;
				MainUIPanel.Visibility = Visibility.Visible;
				_service = new ClickupService(_options.ApiKey);
			}
		}

		private void OnSaveNameClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.DataContext is TaskDetail task)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					try
					{
						await _service.UpdateTaskNameAsync(task);
					}
					catch (ApiException ex)
					{
						await VS.MessageBox.ShowErrorAsync("Couldnt save name", ex.Message);
					}
				}).FireAndForget();
			}
		}

		private void OnSaveDescriptionClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.DataContext is TaskDetail task)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					try
					{
						await _service.UpdateTaskDescriptionAsync(task);
					}
					catch (ApiException ex)
					{
						await VS.MessageBox.ShowErrorAsync("Couldnt save description", ex.Message);
					}
				}).FireAndForget();
			}
		}

		private void OnSubtaskButtonClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.DataContext is Subtask subtask)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					try
					{
						var task = await _service.GetTaskAsync(subtask.Id);

						var statuses = await _service.GetAvailableStatusesAsync(task.Space.Id);
						statuses = statuses.Where(x => x.Status != task.Status.Status).ToList();

						await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

						var vm = ProjectsList.DataContext as ProjectsListViewModel;

						task.AvailableStatuses = statuses;
						task.AvailableStatuses.Add(task.Status);

						_taskDetails.Push(vm.SelectedTask);
						vm.SelectedTask = task;
						ProjectsList.TaskDetailView.DataContext = task;
					}
					catch (Exception e)
					{
						await VS.MessageBox.ShowErrorAsync("Something Went Wrong", e.Message);
					}
				}).FireAndForget();
			}
		}

		private void OnBackButtonClicked(object sender, RoutedEventArgs e)
		{
			if (_taskDetails.Count == 0)
			{
				ProjectsList.DetailedView.Visibility = Visibility.Collapsed;
				ProjectsList.ListView.Visibility = Visibility.Visible;
			}
			else
			{
				var taskDetail = _taskDetails.Pop();

				var vm = ProjectsList.DataContext as ProjectsListViewModel;

				vm.SelectedTask = taskDetail;
				ProjectsList.TaskDetailView.DataContext = taskDetail;
			}

		}

		private void OnStatusChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox && comboBox.DataContext is TaskDetail taskDetail && e.AddedItems.Count > 0)
			{
				if (e.AddedItems[0] is TaskStatus selectedStatus && taskDetail.Status.Status != selectedStatus.Status)
				{
					ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
					{
						try
						{
							await _service.UpdateTaskStatusAsync(taskDetail.Id, selectedStatus.Status);
							taskDetail.Status = selectedStatus;
						}
						catch (Exception ex)
						{
							comboBox.SelectedItem = taskDetail.Status;
							await VS.MessageBox.ShowErrorAsync(Name, $"Failed to update task status: {ex.Message}");
						}
					}).FireAndForget();
				}
			}
		}

		private void OnAddTask(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(ProjectsList.TaskDetailView.CreateTaskTextBox.Text) && ProjectsList.TaskDetailView.DataContext is TaskDetail detail)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var listId = detail.List.Id;

					var result = await _service.CreateTaskAsync(listId, new()
					{
						Name = ProjectsList.TaskDetailView.CreateTaskTextBox.Text,
						Parent = detail.Id
					});

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					ProjectsList.TaskDetailView.CreateTaskTextBox.Text = "";
					detail.Subtasks.Add(new()
					{
						Id = result.Id,
						Name = result.Name,
						Status = result.Status,
						Priority = result.Priority,
					});

				}).FireAndForget();
			}
		}


		private void OnAddTaskItem(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.DataContext is Checklist checklist && !string.IsNullOrEmpty(checklist.NewItemText))
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var item = await _service.CreateChecklistItemAsync(checklist.Id, checklist.NewItemText);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					checklist.Items.Add(item);

					checklist.NewItemText = "";

				}).FireAndForget();
			}
		}

		private void OnCheckChanged(object sender, RoutedEventArgs e)
		{
			if (sender is CheckBox checkBox && checkBox.DataContext is ChecklistItem checklistItem)
			{
				checklistItem.Resolved = checkBox.IsChecked ?? false;
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					await _service.UpdateChecklistItemAsync(checklistItem.ChecklistId, checklistItem.Id, checklistItem);
				}).FireAndForget();
			}
		}

		private void OnDeleteComment(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem menuItem && menuItem.DataContext is Comment comment)
			{
				string commentId = comment.Id;
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					await _service.DeleteCommentAsync(commentId);
					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
					var taskDetail = ProjectsList.TaskDetailView.DataContext as TaskDetail;
					taskDetail.Comments.Remove(comment);
				}).FireAndForget();
			}
		}

		private void OnSendComment(object sender, RoutedEventArgs e)
		{
			string comment = ProjectsList.TaskDetailView.CommentInput.Text;

			if (!string.IsNullOrEmpty(comment))
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var commentModel = await _service.CreateTaskCommentAsync((ProjectsList.DataContext as ProjectsListViewModel).SelectedTask.Id, comment);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					ProjectsList.TaskDetailView.CommentInput.Text = "";
					(ProjectsList.TaskDetailView.DataContext as TaskDetail).Comments.Add(commentModel);
				}).FireAndForget();
			}
		}

		private void OnTaskButtonClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is TaskItem taskItem)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var taskDetail = await _service.GetTaskAsync(taskItem.Id);

					var statuses = await _service.GetAvailableStatusesAsync(taskDetail.Space.Id);
					statuses = statuses.Where(x => x.Status != taskDetail.Status.Status).ToList();

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					var vm = ProjectsList.DataContext as ProjectsListViewModel;

					taskDetail.AvailableStatuses = statuses;
					taskDetail.AvailableStatuses.Add(taskDetail.Status); // for some reason the combo box only works if the selected object is in list, also why doesnt clickup do statuses by id?? like why have status id if youre only gonna go by name
					vm.SelectedTask = taskDetail;
					ProjectsList.TaskDetailView.DataContext = taskDetail;
					ProjectsList.DetailedView.Visibility = Visibility.Visible;
					ProjectsList.ListView.Visibility = Visibility.Collapsed;
					
				}).FireAndForget();
			}
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			VS.MessageBox.Show("ClickUpVS", "Button clicked");
		}

		private async void SaveApiKeyButton_Click(object sender, RoutedEventArgs e)
		{
			var key = ApiKeyInput.Text.Trim();

			if (string.IsNullOrEmpty(key))
			{
				VS.MessageBox.ShowError("Please enter a valid key", "key cannot be empty");
				return;
			}

			_options.ApiKey = key;
			await _options.SaveAsync();

			ApiKeyPromptPanel.Visibility = Visibility.Collapsed;
			MainUIPanel.Visibility = Visibility.Visible;
			_service = new ClickupService(_options.ApiKey);

			ThreadHelper.JoinableTaskFactory.RunAsync(async () => await LoadDataAsync()).FireAndForget();
		}

		private async Task LoadDataAsync()
		{
			try
			{
				var workspaces = await _service.GetWorkSpacesAsync();

				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				WorkspaceSelector.WorkspaceComboBox.ItemsSource = workspaces;

				if (workspaces.Count > 0)
				{
					WorkspaceSelector.WorkspaceComboBox.SelectedIndex = 0;

					ThreadHelper.JoinableTaskFactory.RunAsync(async () => await LoadSpacesAsync(workspaces[0].Id)).FireAndForget();
				}
			}
			catch (Exception ex)
			{
				ex.Log();
			}
		}

		private async Task LoadSpacesAsync(string workspaceId)
		{
			var spaces = await _service.GetSpacesAsync(workspaceId);

			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			SpaceList.SpacesTree.ItemsSource = spaces;
		}

		private void ClickUp_Loaded(object sender, RoutedEventArgs e)
		{
			if (_service is null) return;

			ThreadHelper.JoinableTaskFactory.RunAsync(async () => await LoadDataAsync()).FireAndForget();
		}

		private void OnWorkspaceSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			VS.MessageBox.Show("on workspace selection changed");
		}

		private void OnSpaceSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var item = e.NewValue;

			if (item is Folder folder)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var list = await _service.GetListWithTasksAsync(folder.Id);

					var viewModel = new ProjectsListViewModel(list);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					ProjectsList.DataContext = viewModel;
				}).FireAndForget();
			}
		}
	}
}