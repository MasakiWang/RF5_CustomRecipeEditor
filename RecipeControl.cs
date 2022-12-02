namespace RF5_CustomRecipeEditor
{
    public partial class RecipeControl : UserControl
    {
        RecipeViewModel recipeViewModel;
        readonly ComboBox[] materialComboBoxes;

        public RecipeControl()
        {
            InitializeComponent();
            this.materialComboBoxes = new ComboBox[]
            {
                this.comboBoxMaterial1,
                this.comboBoxMaterial2,
                this.comboBoxMaterial3,
                this.comboBoxMaterial4,
                this.comboBoxMaterial5,
                this.comboBoxMaterial6,
            };

            this.comboBoxCategory.FormattingEnabled = false;
            this.comboBoxCategory.Sorted = false;
            this.comboBoxCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxCategory.DataSource = Enum.GetValues(typeof(CraftCategoryId));
            this.comboBoxResultItem.DisplayMember = "Name";
            this.comboBoxResultItem.FormattingEnabled = false;
            this.comboBoxResultItem.Sorted = false;
            this.comboBoxResultItem.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxResultItem.DataSource = ItemDataTable.Instance.Items;

            this.numericUpDownLevel.Minimum = 1;
            this.numericUpDownLevel.Maximum = 99;
            this.numericUpDownLevel.Increment = 1;

            foreach (var comboBox in this.materialComboBoxes)
            {
                comboBox.DisplayMember = "Name";
                comboBox.FormattingEnabled = false;
                comboBox.Sorted = false;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.DataSource = ItemDataTable.Instance.Items;
            }
        }

        public void SetViewModel(RecipeViewModel recipeViewModel)
        {
            this.recipeViewModel = recipeViewModel;

            this.comboBoxCategory.SelectedItem = recipeViewModel.CraftCategoryId;
            this.comboBoxResultItem.SelectedIndex = ItemDataTable.Instance.IndexOf(recipeViewModel.ResultItemID);
            this.numericUpDownLevel.Value = recipeViewModel.Level;

            for (int i = 0; i < recipeViewModel.MaterialCount; i++)
                this.materialComboBoxes[i].SelectedIndex = ItemDataTable.Instance.IndexOf(recipeViewModel.GetMaterial(i).id);
        }
    }
}
