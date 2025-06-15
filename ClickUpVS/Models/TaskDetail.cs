using ClickUpVS.Views.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickUpVS.Models
{
	internal class TaskDetail : INotifyPropertyChanged
	{
		private string _description;
		private string _initialDescription;
		private bool descriptionChanged = false;

		private string _name;
		private string _initialName;
		private bool nameChanged = false;

		public string InitialTitle
		{
			get { return _initialName; }
			set { _initialName = value; }
		}

		public bool NameChanged
		{
			get
			{
				return nameChanged;
			}
			set
			{
				if (nameChanged != value)
				{
					nameChanged = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string InitialDescription
		{
			get { return _initialDescription; }
			set { _initialDescription = value; }
		}

		[JsonIgnore]
		public bool DescriptionChanged
		{
			get
			{
				return descriptionChanged;
			}
			set
			{
				if (descriptionChanged != value)
				{
					descriptionChanged = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string Id { get; set; }
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					NameChanged = _name != _initialName;
				}
			}
		}
		public string TextContent { get; set; }
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if (_description != value)
				{
					_description = value;
					DescriptionChanged = _description != _initialDescription;
				}
			}
		}
		public int? TimeEstimate { get; set; }

		private TaskStatus status;
		public TaskStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
				NotifyPropertyChanged();
			}
		}

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
		public List<Attachment> Attachments { get; set; }

		public ObservableCollection<Subtask> Subtasks { get; set; }

		public ListPreviewModel List { get; set; }
		public SpaceModel Space { get; set; }

		[JsonIgnore]
		public List<TaskStatus> AvailableStatuses { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	internal class SpaceModel
	{
		public string Id { get; set; }
	}

	internal class ListModel
	{
		public string Id { get; set; }
	}

	internal class ListPreviewModel
	{
		public string Id { get; set; }
	}

	internal class Attachment
	{
		public string Title { get; set; }
		public string Url { get; set; }

		[JsonProperty("url_w_query")]
		public string QueryUrl { get; set; }
	}

	internal class PriorityModel
	{
		public string Color { get; set; }
		public string Priority { get; set; }
	}

	internal class Checklist : INotifyPropertyChanged
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<ChecklistItem> Items { get; set; }

		[JsonIgnore]
		private string newItemText = "";

		[JsonIgnore]
		public string NewItemText
		{
			get
			{
				return this.newItemText;
			}

			set
			{
				this.newItemText = value;
				NotifyPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	internal class ChecklistItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public bool Resolved { get; set; }
		public string ChecklistId { get; set; }
	}

	internal class Tag
	{
		public string Name { get; set; }
		public string TagFg { get; set; }
	}
}
