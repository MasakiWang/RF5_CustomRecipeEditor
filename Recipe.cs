using Newtonsoft.Json;

[JsonObject]
public sealed class Recipe
{
    [JsonIgnore]
    public CraftCategoryId CraftCategoryId { get; set; }

    [JsonRequired]
    [JsonProperty]
    public byte CraftCategoryID
    {
        get => (byte)CraftCategoryId;
        set => CraftCategoryId = (CraftCategoryId)value;
    }

    [JsonRequired]
    [JsonProperty]
    public ushort ResultItemID { get; set; }

    [JsonRequired]
    [JsonProperty]
    public List<ushort> IngredientItemIDs { get; set; } = new List<ushort>();

    [JsonProperty]
    public byte SkillLevel { get; set; }
}