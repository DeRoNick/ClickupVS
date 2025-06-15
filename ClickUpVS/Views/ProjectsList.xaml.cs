using ClickUpVS.Models;
using ClickUpVS.Views.Models;
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
	/// Interaction logic for ProjectsList.xaml
	/// </summary>
	public partial class ProjectsList : UserControl
	{
		public event EventHandler<RoutedEventArgs> ButtonClicked;
		public event EventHandler<RoutedEventArgs> BackButtonClicked;
		public event EventHandler<RoutedEventArgs> CreateTaskClicked;

		public ProjectsList()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ButtonClicked?.Invoke(sender, e);
        }

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			BackButtonClicked?.Invoke(sender, e);
		}

		private void CreateTask_Click(object sender, RoutedEventArgs e)
		{
			CreateTaskClicked?.Invoke(sender, e);
        }
    }
}
