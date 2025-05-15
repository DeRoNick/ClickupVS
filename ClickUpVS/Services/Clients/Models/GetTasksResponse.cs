using ClickUpVS.Models;
using System.Collections.Generic;

namespace ClickUpVS.Services.Clients.Models
{
	internal class GetTasksResponse
	{
		public List<TaskItem> Tasks { get; set; }
	}
}
