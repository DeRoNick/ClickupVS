namespace ClickUpVS.Models
{
	internal class TaskStatus
	{
		public string Id { get; set; }
		public string Status { get; set; }
		public string Color { get; set; }
	}

	internal class TaskItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public TaskStatus Status { get; set; }
	}
}
