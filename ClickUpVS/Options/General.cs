using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ClickUpVS
{
	internal partial class OptionsProvider
	{
		// Register the options with this attribute on your package class:
		// [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "ClickUpVS", "General", 0, 0, true, SupportsProfiles = true)]
		[ComVisible(true)]
		public class GeneralOptions : BaseOptionPage<General> { }
	}

	public class General : BaseOptionModel<General>
	{
		[Category("General")]
		[DisplayName("Api Key")]
		[Description("API key needed to use ClickUp.")]
		[DefaultValue("")]
		public string ApiKey { get; set; }
	}
}
