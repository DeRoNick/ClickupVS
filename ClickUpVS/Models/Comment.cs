using ClickUpVS.Views.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ClickUpVS.Models
{
	internal class ReactionModel
	{
		public string Reaction { get; set; }

		public string ReactionEmoji => ConvertEmojiHex(Reaction);

		[JsonConverter(typeof(UnixMillisecondDateTimeConverter))]
		public DateTime Date { get; set; }
		public User User { get; set; }

		private static string ConvertEmojiHex(string hex)
		{
			// Convert the hex string to an integer (Unicode code point)
			int codePoint = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			return char.ConvertFromUtf32(codePoint);
		}
	}

	internal class Comment
	{
		public string Id { get; set; }
		public string CommentText { get; set; }
		public User User { get; set; }
		public List<ReactionModel> Reactions { get; set; }

		[JsonConverter(typeof(UnixMillisecondDateTimeConverter))]
		public DateTime Date { get; set; }

		public bool Deletable { get; set; }
	}
}
