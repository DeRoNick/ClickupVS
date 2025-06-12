using ClickUpVS.Views.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClickUpVS.Models
{
	internal class TaskDetail
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string TextContent { get; set; }
		public string Description { get; set; }
		public int TimeEstimate { get; set; }
		public TaskStatus Status { get; set; }

		[JsonConverter(typeof(UnixMillisecondDateTimeConverter))]
		public DateTime? DueDate { get; set; }
		[JsonConverter(typeof(UnixMillisecondDateTimeConverter))]

		public DateTime? StartDate { get; set; }
		public int? Points { get; set; }
		public List<Tag> Tags { get; set; }
		public string Url { get; set; }
		public List<Checklist> Checklists { get; set; }

		public ObservableCollection<Comment> Comments { get; set; }
		public List<User> Assignees { get; set; }
		public PriorityModel Priority { get; set; }
	}

	internal class PriorityModel
	{
		public string Color { get; set; }
		public string Priority { get; set; }
	}

	internal class Checklist
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<ChecklistItem> Items { get; set; }
	}

	internal class ChecklistItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public bool Resolved { get; set; }
	}

	internal class Tag
	{
		public string Name { get; set; }
		public string TagFg { get; set; }
	}
}
