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

        ImmutableDictionary<ushort, ItemDataRow> items;

        public IList<ItemDataRow> Items => items.Values.ToList();

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

                    items = csvReader.GetRecords<ItemDataRow>().ToImmutableDictionary(x => x.id);
                }
            }
        }

        public bool Contains (ushort id) => items.ContainsKey(id);

        public ItemDataRow Get(ushort id) => items[id];
    }
}
