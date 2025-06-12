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
    }
}
