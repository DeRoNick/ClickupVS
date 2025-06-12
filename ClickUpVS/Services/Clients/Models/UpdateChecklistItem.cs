using ClickUpVS.Models;

namespace ClickUpVS.Services.Clients.Models
{
	internal class UpdateChecklistItemRequest
	{
		public string Name { get; set; }
		public bool Resolved { get; set; }
	}

	internal class CreateChecklistItemRequest
	{
		public string Name { get; set; }
	}

	internal class CreateChecklistItemResponse
	{
		public Checklist Checklist { get; set; }
	}
}
