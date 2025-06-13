namespace ClickUpVS.Services.Clients.Models
{
	internal class CreateTaskRequest
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Parent { get; set; }
	}
}
