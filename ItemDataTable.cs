using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Immutable;
using System.Globalization;

namespace RF5_CustomRecipeEditor
{
    public sealed class ItemDataTable
    {
        private static ItemDataTable? instance = null;
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

                    itemsArray = csvReader.GetRecords<ItemDataRow>().OrderBy(x => x.id).ToImmutableArray();
                    itemsDict = itemsArray.ToImmutableDictionary(x => x.id);
                }
            }

            itemIndex = itemsArray
                .Select((x, i) => (index: i, id: x.id))
                .ToImmutableDictionary(t => t.id, t => t.index);
        }

        public bool Contains (ushort id) => itemsDict.ContainsKey(id);

        public ItemDataRow? Get(ushort id)
        {
            if (itemsDict.TryGetValue(id, out var value))
                return value;

            return null;
        }

        public int IndexOf(ushort id)
        {
            return itemIndex[id];
        }
    }
}
