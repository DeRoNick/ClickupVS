using System.Windows.Controls;

namespace ClickUpVS.Views
{
	/// <summary>
	/// Interaction logic for WorkspaceSelector.xaml
	/// </summary>
	public partial class WorkspaceSelector : UserControl
	{
		public event EventHandler<System.Windows.Controls.SelectionChangedEventArgs> SelectionChanged;

		public WorkspaceSelector()
		{
			InitializeComponent();
		}

		private void WorkspaceComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			SelectionChanged?.Invoke(sender, e);
		}
	}
}
