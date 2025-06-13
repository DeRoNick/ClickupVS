using ClickUpVS.Views.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ClickUpVS.Models
{
	internal class Subtask
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public TaskStatus Status { get; set; }
		public List<User> Assignees { get; set; }
		public PriorityModel Priority { get; set; }

		[JsonConverter(typeof(UnixMillisecondDateTimeConverter))]
		public DateTime? DueDate { get; set; }
	}
}
