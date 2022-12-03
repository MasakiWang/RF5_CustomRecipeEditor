using System.Data;

namespace RF5_CustomRecipeEditor
{
    public partial class ItemSelectionBox : Form
    {
        static ItemSelectionBox Instance = new ItemSelectionBox();

        public ItemDataRow? Value { get; private set; } = null;

        private ItemSelectionBox()
        {
            InitializeComponent();

            this.comboBox.DataSource = ItemDataTable.Instance.Items;
            this.comboBox.AutoCompleteMode= AutoCompleteMode.SuggestAppend;
            this.comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.comboBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            this.comboBox.AutoCompleteCustomSource.AddRange(ItemDataTable.Instance.Items.Select(x => x.Name).ToArray());

            this.buttonAccept.Click += (_, _) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Value = (ItemDataRow)comboBox.SelectedItem;
                this.Hide();
            };
            this.buttonCancel.Click += (_, args) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Value = null;
                this.Hide();
            };

            this.Visible = false;
        }

        private new void Hide()
        {
            MainForm.Instance!.Enabled = true;
            MainForm.Instance!.Activate();
            this.Visible = false;
        }

        public static async Task<(DialogResult result, ItemDataRow? value)> Show(Control parent, ushort id)
        {
            MainForm.Instance!.Enabled = false;

            Instance.comboBox.SelectedIndex = ItemDataTable.Instance.IndexOf(id);
            var pos = parent.PointToScreen(Point.Empty);
            pos.Offset(-14, -12);
            Instance.Location = pos;
            Instance.Show();

            while (Instance.Visible)
                await Task.Delay(16);

            var result = Instance.DialogResult;
            var value = Instance.Value;

            return (result, value);
        }
    }
}