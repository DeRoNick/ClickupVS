﻿using ClickUpVS.Models;
using ClickUpVS.Services;
using ClickUpVS.Views;
using ClickUpVS.Views.Models;
using Microsoft.VisualStudio.Threading;
using RestEase;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
			ProjectsList.TaskDetailView.OnPriorityChanged += OnPriorityChanged;
			ProjectsList.CreateTaskClicked += OnCreateTask;
			ProjectsList.TaskDetailView.OnCreateChecklistClicked += OnCreateChecklist;
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

		private void OnCreateChecklist(object sender, RoutedEventArgs e)
		{
			var name = ProjectsList.TaskDetailView.CreateChecklistTextBox.Text;
			if (sender is Button button && button.Tag is string taskId && !string.IsNullOrEmpty(name))
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var checklist = await _service.CreateChecklistAsync(taskId, name);

					var task = ProjectsList.TaskDetailView.DataContext as TaskDetail;
					ProjectsList.TaskDetailView.CreateChecklistTextBox.Text = string.Empty;
					task.Checklists.Add(checklist);
				}).FireAndForget();
			}
		}

		private void OnCreateTask(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is string projectId)
			{
				var textBox = FindChild<TextBox>(ProjectsList, x => x.Name == "CreateTaskTextBox" && x.Tag is string s && s == projectId);
				if (textBox is null || string.IsNullOrEmpty(textBox.Text))
					return;
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					try
					{
						var task = await _service.CreateTaskAsync(projectId, new() { Name = textBox.Text });

						var vm = ProjectsList.DataContext as ProjectsListViewModel;
						vm.AddTask(task);
						textBox.Text = string.Empty;
					}
					catch (ApiException ex)
					{
						await VS.MessageBox.ShowErrorAsync("Couldnt create task", ex.Message);
					}
				}).FireAndForget();
			}
		}

		private static T FindChild<T>(DependencyObject parent, Func<T, bool> predicate = null) where T : DependencyObject
		{
			if (parent == null) return null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child is T typedChild && (predicate == null || predicate(typedChild)))
				{
					return typedChild;
				}

				var result = FindChild(child, predicate);
				if (result != null)
					return result;
			}

			return null;
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

						var statuses = await _service.GetAvailableStatusesAsync(task.List.Id);

						await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

						var vm = ProjectsList.DataContext as ProjectsListViewModel;

						task.AvailableStatuses = statuses;

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

		private void OnPriorityChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox && comboBox.DataContext is TaskDetail taskDetail && e.AddedItems.Count > 0)
			{
				if (e.AddedItems[0] is PriorityModel model && taskDetail.Priority != model)
				{
					ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
					{
						try
						{
							await _service.UpdateTaskPriorityAsync(taskDetail.Id, model.Priority);
							taskDetail.Priority = model;
						}
						catch (ApiException ex)
						{
							comboBox.SelectedItem = taskDetail.Priority;
							await VS.MessageBox.ShowErrorAsync("Failed to change priority", ex.Message);
						}
					}).FireAndForget();
				}
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

					var statuses = await _service.GetAvailableStatusesAsync(taskDetail.List.Id);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					var vm = ProjectsList.DataContext as ProjectsListViewModel;

					taskDetail.AvailableStatuses = statuses; // why doesnt clickup do statuses by id?? like why have status id if youre only gonna go by name
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
			if (sender is ComboBox box && box.SelectedItem is Workspace workspace)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () => await LoadSpacesAsync(workspace.Id)).FireAndForget();
			}
		}

		private void OnSpaceSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var item = e.NewValue;

			if (item is Folder folder)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var list = await _service.GetListsWithTasksAsync(folder.Id);

					var viewModel = new ProjectsListViewModel(list);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					ProjectsList.DataContext = viewModel;
				}).FireAndForget();
			}
			else if (item is List list)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var listWithTasks = await _service.GetListWithTasksAsync(list.Id);

					var viewModel = new ProjectsListViewModel([listWithTasks]);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					ProjectsList.DataContext = viewModel;
				}).FireAndForget();
			}
		}
	}
}