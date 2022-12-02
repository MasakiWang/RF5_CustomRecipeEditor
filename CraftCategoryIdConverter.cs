using Newtonsoft.Json;

namespace RF5_CustomRecipeEditor
{
    public class CraftCategoryIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.Value)
            {
                case string str: return (byte)Enum.Parse<CraftCategoryId>(str);
                case int i: return (byte)i;
                case long i: return (byte)i;
                case byte i: return (byte)i;
            }

            return 0;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue((byte)value!);
        }
    }
}
