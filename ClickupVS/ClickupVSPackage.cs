﻿global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using RestEase;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
[assembly: InternalsVisibleTo(RestClient.FactoryAssemblyName)]

namespace ClickUpVS
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
	[ProvideToolWindow(typeof(ClickUpWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.SolutionExplorer)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideBindingPath]
	[Guid(PackageGuids.ClickUpVSString)]
	[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "ClickUpVS", "General", 0, 0, true, SupportsProfiles = true)]
	public sealed class ClickUpVSPackage : ToolkitPackage
	{
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await this.RegisterCommandsAsync();

			this.RegisterToolWindows();
		}
	}
}