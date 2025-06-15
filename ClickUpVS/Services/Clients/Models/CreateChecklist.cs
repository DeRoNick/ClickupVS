using ClickUpVS.Models;

namespace ClickUpVS.Services.Clients.Models
{
	internal class CreateChecklistRequest
	{
		public string Name { get; set; }
	}

	internal class CreateChecklistResponse
	{
		public Checklist Checklist { get; set; }
	}
}
