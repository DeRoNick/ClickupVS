namespace ClickUpVS
{
	[Command(PackageIds.OpenWindow)]
	internal sealed class OpenWindowCommand : BaseCommand<OpenWindowCommand>
	{
		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			await ClickUpWindow.ShowAsync();
		}
	}
}
