using ClickUpVS.Services;
using Microsoft.VisualStudio.Threading;
using System.Threading.Tasks;
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
			WorkspaceSelector.SelectionChanged += OnSelectionChanged;
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
				}
			}
			catch (Exception ex)
			{
				ex.Log();
			}
		}

		private async void ClickUp_Loaded(object sender, RoutedEventArgs e)
		{
			if (_service is null) return;

			ThreadHelper.JoinableTaskFactory.RunAsync(async () => await LoadDataAsync()).FireAndForget();
		}

		private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			VS.MessageBox.Show("on selection changed");
		}
	}
}