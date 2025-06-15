using ClickUpVS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClickUpVS.Views
{
	/// <summary>
	/// Interaction logic for TaskDetailView.xaml
	/// </summary>
	public partial class TaskDetailView : UserControl
	{
		public event EventHandler<RoutedEventArgs> OnSendComment;
		public event EventHandler<RoutedEventArgs> OnDeleteComment;
		public event EventHandler<RoutedEventArgs> OnCheckChanged;
		public event EventHandler<RoutedEventArgs> OnAddTaskItem;
		public event EventHandler<RoutedEventArgs> OnAddTask;
		public event EventHandler<System.Windows.Controls.SelectionChangedEventArgs> OnStatusChanged;
		public event EventHandler<RoutedEventArgs> OnSubtaskButtonClicked;

		public TaskDetailView()
		{
			InitializeComponent();
		}


		private void SendComment_Click(object sender, RoutedEventArgs e)
		{
			OnSendComment?.Invoke(sender, e);
        }

		private void DeleteComment_Click(object sender, RoutedEventArgs e)
		{
			OnDeleteComment?.Invoke(sender, e);
		}

		private void CheckBox_CheckChanged(object sender, RoutedEventArgs e)
		{
			OnCheckChanged?.Invoke(sender, e);
        }

		private void File_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// use this if cant publish
			//if (sender is StackPanel panel && panel.DataContext is Attachment attachment)
			//{
			//	VS.MessageBox.Show($"Visual studio extensions dont allow opening attachments directly from the extension for security reasons. Please copy the link and open it in your browser.\n {attachment.Url}");
			//}

			// this might make the extension unpublishable because it starts a process
			// so its a security risk, lets leave it in for now

			if (sender is StackPanel panel && panel.DataContext is Attachment attachment)
			{
				try
				{
					System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(string.IsNullOrEmpty(attachment.QueryUrl) ? attachment.Url : attachment.QueryUrl)
					{
						UseShellExecute = true
					});
				}
				catch (Exception ex)
				{
					VS.MessageBox.Show("Failed to open link: " + ex.Message);
				}
			}
		}

		private void AddTaskItem_Click(object sender, RoutedEventArgs e)
		{
			OnAddTaskItem?.Invoke(sender, e);
		}

		private void AddTask_Click(object sender, RoutedEventArgs e)
		{
			OnAddTask?.Invoke(sender, e);
		}

		private void StatusComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			OnStatusChanged?.Invoke(sender, e);
		}

		private void SubtaskButton_Click(object sender, RoutedEventArgs e)
		{
			OnSubtaskButtonClicked?.Invoke(sender, e);
		}
	}
}
