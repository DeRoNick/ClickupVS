using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClickUpVS
{
	public partial class ClickUpWindowControl : UserControl
	{
		private General _options;

		public ClickUpWindowControl()
		{
			InitializeComponent();
			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			_options = await General.GetLiveInstanceAsync();

			if (string.IsNullOrEmpty(_options.ApiKey))
			{
				ApiKeyPromptPanel.Visibility = Visibility.Visible;
				ClickMe.Visibility = Visibility.Collapsed;
			}
			else
			{
				ApiKeyPromptPanel.Visibility = Visibility.Collapsed;
				ClickMe.Visibility = Visibility.Visible;
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
			ClickMe.Visibility = Visibility.Visible;
		}
	}
}