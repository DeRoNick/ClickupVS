using System.Collections.Generic;

namespace ClickUpVS.Models
{
	internal class TaskStatus
	{
		public string Id { get; set; }
		public string Status { get; set; }
		public string Color { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is TaskStatus status)
			{
				return status.Id == this.Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
		}
	}

	internal class TaskItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public TaskStatus Status { get; set; }
	}
}
