using System.Collections.Immutable;

namespace RF5_CustomRecipeEditor
{
    public partial class RecipeControl : UserControl
    {
        public event EventHandler<Recipe> ClickDuplicate = delegate { };
        public event EventHandler<Recipe> ClickRemove = delegate { };

        RecipeViewModel? recipeViewModel = null;
        readonly ImmutableArray<ComboBox> materialComboBoxes;

        volatile bool initailzing = false;

        public RecipeControl()
        {
            InitializeComponent();
            this.materialComboBoxes = new []
            {
                this.comboBoxMaterial1,
                this.comboBoxMaterial2,
                this.comboBoxMaterial3,
                this.comboBoxMaterial4,
                this.comboBoxMaterial5,
                this.comboBoxMaterial6,
            }.ToImmutableArray();

            this.comboBoxCategory.DataSource = Enum.GetValues(typeof(CraftCategoryId));
            this.comboBoxCategory.SelectedIndexChanged += (_, _) =>
            {
                if (initailzing)
                    return;

                this.recipeViewModel!.CraftCategoryId = (CraftCategoryId)this.comboBoxCategory.SelectedItem;
            };

            this.comboBoxResultItem.DataSource = ItemDataTable.Instance.Items;
            this.comboBoxResultItem.SelectedIndexChanged += (_, _) =>
                this.recipeViewModel!.ResultItemID = ((ItemDataRow)this.comboBoxResultItem.SelectedItem).id;

            this.numericUpDownLevel.Minimum = 1;
            this.numericUpDownLevel.Maximum = 99;
            this.numericUpDownLevel.Increment = 1;
            this.numericUpDownLevel.ValueChanged += (_, _) =>
            {
                if (initailzing)
                    return;

                this.recipeViewModel!.Level = (byte)this.numericUpDownLevel.Value;
            };

            for (int i = 0; i < this.materialComboBoxes.Length; i++)
            {
                var index = i;
                var comboBox = this.materialComboBoxes[i];
                comboBox.DataSource = ItemDataTable.Instance.Items;
                comboBox.SelectedIndexChanged += (_, _) =>
                {
                    if (initailzing)
                        return;

                    this.recipeViewModel!.SetMaterial(index, (ItemDataRow)comboBox.SelectedItem);
                };
            }
        }

        public void SetViewModel(RecipeViewModel recipeViewModel)
        {
            this.initailzing = true;

            try
            {
                this.recipeViewModel = recipeViewModel;

                this.comboBoxCategory.SelectedItem = recipeViewModel.CraftCategoryId;
                this.comboBoxResultItem.SelectedIndex = ItemDataTable.Instance.IndexOf(recipeViewModel.ResultItemID);
                this.numericUpDownLevel.Value = recipeViewModel.Level;

                int i = 0;
                for (; i < recipeViewModel.MaterialCount; i++)
                    this.materialComboBoxes[i].SelectedIndex = ItemDataTable.Instance.IndexOf(recipeViewModel.GetMaterial(i)?.id ?? 0);

                for (; i < 6; i++)
                    this.materialComboBoxes[i].SelectedIndex = 0;
            }
            finally
            {
                initailzing = false;
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            ClickDuplicate?.Invoke(this, recipeViewModel!.Recipe);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            ClickRemove?.Invoke(this, recipeViewModel!.Recipe);
        }
    }
}
