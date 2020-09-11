using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GamingStore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GamingStore.Contracts.Converters
{
    public class DictionaryJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            Type type = value.GetType();
            jo.Add("type", type.Name);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead)
                {
                    object propVal = prop.GetValue(value, null);
                    if (propVal != null)
                    {
                        jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
                    }
                }
            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Item).IsAssignableFrom(objectType);
        }
    }
}