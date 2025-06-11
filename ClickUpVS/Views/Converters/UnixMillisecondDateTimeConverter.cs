using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace ClickUpVS.Views.Converters
{
	internal class UnixMillisecondDateTimeConverter : DateTimeConverterBase
	{
		internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		//
		// Summary:
		//     Writes the JSON representation of the object.
		//
		// Parameters:
		//   writer:
		//     The Newtonsoft.Json.JsonWriter to write to.
		//
		//   value:
		//     The value.
		//
		//   serializer:
		//     The calling serializer.
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			long num;
			if (value is DateTime dateTime)
			{
				num = (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
			}
			else
			{
				if (!(value is DateTimeOffset dateTimeOffset))
				{
					throw new JsonSerializationException("Expected date object value.");
				}

				num = (long)(dateTimeOffset.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
			}

			if (num < 0)
			{
				throw new JsonSerializationException("Cannot convert date value that is before Unix epoch of 00:00:00 UTC on 1 January 1970.");
			}

			writer.WriteValue(num);
		}

		//
		// Summary:
		//     Reads the JSON representation of the object.
		//
		// Parameters:
		//   reader:
		//     The Newtonsoft.Json.JsonReader to read from.
		//
		//   objectType:
		//     Type of the object.
		//
		//   existingValue:
		//     The existing property value of the JSON that is being converted.
		//
		//   serializer:
		//     The calling serializer.
		//
		// Returns:
		//     The object value.
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = Nullable.GetUnderlyingType(objectType) != null;
			if (reader.TokenType == JsonToken.Null)
			{
				if (!flag)
				{
					throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert null value to {0}.", objectType));
				}

				return null;
			}

			long result;
			if (reader.TokenType == JsonToken.Integer)
			{
				result = (long)reader.Value;
			}
			else
			{
				if (reader.TokenType != JsonToken.String)
				{
					throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected token parsing date. Expected Integer or String, got {0}.", reader.TokenType));
				}

				if (!long.TryParse((string)reader.Value, out result))
				{
					throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert invalid value to {0}.", objectType));
				}
			}

			if (result >= 0)
			{
				DateTime dateTime = UnixEpoch.AddMilliseconds(result);
				if ((flag ? Nullable.GetUnderlyingType(objectType) : objectType) == typeof(DateTimeOffset))
				{
					return new DateTimeOffset(dateTime, TimeSpan.Zero);
				}

				return dateTime;
			}

			throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert value that is before Unix epoch of 00:00:00 UTC on 1 January 1970 to {0}.", objectType));
		}
	}
}
