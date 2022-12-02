using Newtonsoft.Json;

namespace RF5_CustomRecipeEditor
{
    public sealed class ItemIDConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.Value)
            {
                case string str: return (ushort)Enum.Parse<ItemID>(str);
                case int i: return (ushort)i;
                case long i: return (ushort)i;
            }

            return 0;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue((ushort)value!);
        }
    }
}
