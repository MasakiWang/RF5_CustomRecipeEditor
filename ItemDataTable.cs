using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Immutable;
using System.Globalization;

namespace RF5_CustomRecipeEditor
{
    public sealed class ItemDataTable
    {
        private static ItemDataTable instance;
        public static ItemDataTable Instance
        {
            get
            {
                if (null == instance)
                    instance = new ItemDataTable();
                return instance;
            }
        }

        ImmutableDictionary<ushort, ItemDataRow> itemsDict;
        ImmutableDictionary<ushort, int> itemIndex;
        ImmutableArray<ItemDataRow> itemsArray;

        public IList<ItemDataRow> Items => itemsArray;

        ItemDataTable()
        {
            using (var textReader = new StreamReader("ItemDataTable.csv"))
            {
                using (var csvReader = new CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    NewLine = Environment.NewLine,
                }))
                {
                    if (!csvReader.Read())
                        throw new Exception();
                    if (!csvReader.Read())
                        throw new Exception();

                    if (!csvReader.ReadHeader())
                        throw new Exception();

                    itemsDict = csvReader.GetRecords<ItemDataRow>().ToImmutableDictionary(x => x.id);
                    itemsArray = itemsDict.Values.ToImmutableArray();
                }
            }

            itemIndex = itemsArray
                .Select((x, i) => (index: i, id: x.id))
                .ToImmutableDictionary(t => t.id, t => t.index);
        }

        public bool Contains (ushort id) => itemsDict.ContainsKey(id);

        public ItemDataRow Get(ushort id) => itemsDict[id];

        public int IndexOf(ushort id)
        {
            return itemIndex[id];
        }
    }
}
