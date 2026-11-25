using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PatchKit.Core.CSharp;

public class ImmutableArrayJsonConverter<T> : JsonConverter
{
	public override bool CanRead => true;

	public override bool CanWrite => true;

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (serializer == null)
		{
			throw new ArgumentNullException("serializer");
		}
		if (!(value is ImmutableArray<T> immutableArray))
		{
			throw new ArgumentException("value");
		}
		serializer.Serialize(writer, immutableArray.ToArray());
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JToken jToken = JToken.Load(reader);
		if (jToken != null && jToken.Type == JTokenType.Array)
		{
			List<T> list = jToken.ToObject<List<T>>();
			Assert.IsNotNull(list);
			return list.AsImmutable();
		}
		return ImmutableArray<T>.Empty;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(ImmutableArray<T>);
	}
}
