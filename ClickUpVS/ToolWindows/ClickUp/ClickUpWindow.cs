using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ClickUpVS
{
	public class ClickUpWindow : BaseToolWindow<ClickUpWindow>
	{
		public override string GetTitle(int toolWindowId) => "ClickUp";

		public override Type PaneType => typeof(Pane);

		public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
		{
			return Task.FromResult<FrameworkElement>(new ClickUpWindowControl());
		}

		[Guid("ac028f23-a72a-49e3-81cd-e64f49d0fe2a")]
		internal class Pane : ToolkitToolWindowPane
		{
			public Pane()
			{
				BitmapImageMoniker = KnownMonikers.ToolWindow;
			}
		}
	}
}