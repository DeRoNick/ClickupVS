namespace ClickUpVS
{
	[Command(PackageIds.OpenWindow)]
	internal sealed class OpenWindowCommand : BaseCommand<OpenWindowCommand>
	{
		protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			return ClickUpWindow.ShowAsync();
		}
	}
}
