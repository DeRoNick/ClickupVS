using ClickUpVS.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ClickUpVS.Views.Models
{
	internal class StatusGroup
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public ObservableCollection<TaskItem> Tasks { get; set; }
		public bool IsExpanded { get; set; } = true;
	}

	internal class ProjectModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<StatusGroup> StatusGroups { get; set; }
		public bool IsExpanded { get; set; } = true;
	}

	internal class ProjectsListViewModel
	{
		public List<ProjectModel> Projects { get; set; }
		public TaskDetail SelectedTask { get; set; }

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

		public void AddTask(TaskDetail task)
		{
			var projectModel = Projects.First(x => x.Id == task.List.Id);

			var statusGroup = projectModel.StatusGroups.FirstOrDefault(x => x.Id == task.Status.Id);

			if (statusGroup is null)
			{
				projectModel.StatusGroups.Add(new()
				{
					Id = task.Status.Id,
					Name = task.Status.Status,
					Color = task.Status.Color,
					Tasks = [new () {
						Id = task.Id,
						Name = task.Name,
						Status = task.Status
					}]
				});
			}
			else
			{
				statusGroup.Tasks.Add(new()
				{
					Id = task.Id,
					Name = task.Name,
					Status = task.Status
				});
			}
		}
	}
}
