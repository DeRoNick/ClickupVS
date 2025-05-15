using System.Windows.Controls;

namespace ClickUpVS.Views
{
	/// <summary>
	/// Interaction logic for SpaceList.xaml
	/// </summary>
	public partial class SpaceList : UserControl
	{
		public event EventHandler<System.Windows.RoutedPropertyChangedEventArgs<object>> SelectionChanged;

		public SpaceList()
		{
			InitializeComponent();
		}

		private void SpacesTree_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
		{
			SelectionChanged?.Invoke(this, e);
		}
	}
}
