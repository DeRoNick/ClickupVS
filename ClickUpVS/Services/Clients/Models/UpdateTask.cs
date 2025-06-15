using ClickUpVS.Models;
using Newtonsoft.Json;

namespace ClickUpVS.Services.Clients.Models
{
	internal class UpdateTaskRequest
	{
		public string Description { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Status { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Priorities? Priority { get; set; }
	}
}
