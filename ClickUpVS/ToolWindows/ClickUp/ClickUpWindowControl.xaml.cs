using ClickUpVS.Models;
using ClickUpVS.Services;
using ClickUpVS.Views.Models;
using Microsoft.VisualStudio.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ClickUpVS
{
	public partial class ClickUpWindowControl : UserControl
	{
		private General _options;
		private ClickupService _service;

		public ClickUpWindowControl()
		{
			InitializeComponent();
			_ = InitializeAsync();
			WorkspaceSelector.SelectionChanged += OnWorkspaceSelectionChanged;
			SpaceList.SelectionChanged += OnSpaceSelectionChanged;
			ProjectsList.ButtonClicked += OnTaskButtonClicked;
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

		private void OnTaskButtonClicked(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is TaskItem taskItem)
			{
				ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
				{
					var taskDetail = await _service.GetTaskAsync(taskItem.Id);

					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

					var vm = ProjectsList.DataContext as ProjectsListViewModel;

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