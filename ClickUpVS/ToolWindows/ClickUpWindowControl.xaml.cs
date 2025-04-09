using System.Windows;
using System.Windows.Controls;

namespace ClickUpVS
{
	public partial class ClickUpWindowControl : UserControl
	{
		public ClickUpWindowControl()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			VS.MessageBox.Show("ClickUpVS", "Button clicked");
		}
	}
}