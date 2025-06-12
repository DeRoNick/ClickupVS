using ClickUpVS.Views.Converters;
using Newtonsoft.Json;

namespace ClickUpVS.Services.Clients.Models
{
	internal class CreateTaskCommentRequest
	{
		public string CommentText { get; set; }
		public bool NotifyAll { get; set; } = true;
	}

	internal class CreateTaskCommentResponse
	{
		public string Id { get; set; }

		[JsonConverter(typeof(UnixMillisecondDateTimeConverter))]
		public DateTime Date { get; set; }
	}
}
