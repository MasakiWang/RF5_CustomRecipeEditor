using Newtonsoft.Json;
using RF5_CustomRecipeEditor;

[JsonObject]
public sealed class Recipe
{
    [JsonIgnore]
    public CraftCategoryId CraftCategoryId { get; set; }

    [JsonRequired]
    [JsonProperty]
    [JsonConverter(typeof(CraftCategoryIdConverter))]
    public byte CraftCategoryID
    {
        get => (byte)CraftCategoryId;
        set => CraftCategoryId = (CraftCategoryId)value;
    }

    [JsonRequired]
    [JsonProperty]
    [JsonConverter(typeof(ItemIDConverter))]
    public ushort ResultItemID { get; set; }

    [JsonRequired]
    [JsonProperty(ItemConverterType = typeof(ItemIDConverter))]
    public List<ushort> IngredientItemIDs { get; set; } = new List<ushort>();

    [JsonProperty]
    public byte SkillLevel { get; set; } = 1;
}