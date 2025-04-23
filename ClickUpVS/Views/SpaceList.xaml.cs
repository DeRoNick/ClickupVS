using System.Windows.Controls;

namespace ClickUpVS.Views
{
	/// <summary>
	/// Interaction logic for SpaceList.xaml
	/// </summary>
	public partial class SpaceList : UserControl
	{
		public event EventHandler<System.Windows.Controls.SelectionChangedEventArgs> SelectionChanged;

		public SpaceList()
		{
			InitializeComponent();
		}

		private void SpacesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			SelectionChanged?.Invoke(this, e);
        }
    }
}
