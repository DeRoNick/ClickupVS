using ClickUpVS.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClickUpVS.Views.Models
{
	internal class StatusGroup
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public List<TaskItem> Tasks { get; set; }
	}

	internal class ProjectModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<StatusGroup> StatusGroups { get; set; }
	}

	internal class ProjectsListViewModel
	{
		public List<ProjectModel> Projects { get; set; }

		public ProjectsListViewModel(List<List> lists)
		{
			Projects = [.. lists.Select(x => new ProjectModel
			{
				Id = x.Id,
				Name = x.Name,
				StatusGroups = [.. x.Tasks.GroupBy(x => x.Status.Id)
					.Select(x => new StatusGroup {
						Id = x.Key,
						Name = x.Select(x => x.Status.Status).First(),
						Color = x.Select(x => x.Status.Color).First(),
						Tasks = [.. x.Select(x => x)]
					})]
			})];
		}
	}
}
