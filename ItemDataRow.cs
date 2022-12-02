using CsvHelper.Configuration.Attributes;

public sealed class ItemDataRow
{
    private string displayName;

    [Name("id")]
    public ushort id { get; set; }

    [Name("english_name")]
    public string english_name { get; set; }

    [Name("item_name")]
    public string item_name { get; set; }

    [Ignore]
    public string Name
    {
        get
        {
            if (null == displayName) 
                displayName = $"[{id}] {item_name} | {english_name}";
            return displayName ;
        }
    }

    [Ignore]
    public string json_file_name => english_name.Replace(" ", string.Empty) + ".json";

    public override string ToString()
    {
        return Name;
    }
}