using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ClickUpVS.Models
{
	internal class Space
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<Folder> Folders { get; set; }
		public List<List> Lists { get; set; }

		public List<TaskStatus> Statuses { get; set; }

		public List<object> Children => [.. Folders.Cast<object>(), .. Lists];
	}

	internal class List
	{
		public string Id { get; set; }
		public string Name { get; set; }

		[JsonIgnore]
		public List<TaskItem> Tasks { get; set; }

		public List<Models.TaskStatus> Statuses { get; set; }
	}
}
