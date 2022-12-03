using System.Collections.Immutable;

namespace RF5_CustomRecipeEditor
{
    public partial class RecipeControlv2 : UserControl
    {
        public event EventHandler<Recipe> ClickDuplicate = delegate { };
        public event EventHandler<Recipe> ClickRemove = delegate { };

        RecipeViewModel? recipeViewModel = null;
        readonly ImmutableArray<Button> materialbuttons;

        volatile bool initailzing = false;

        public RecipeControlv2()
        {
            InitializeComponent();
            this.materialbuttons = new []
            {
                this.buttonMaterial1,
                this.buttonMaterial2,
                this.buttonMaterial3,
                this.buttonMaterial4,
                this.buttonMaterial5,
                this.buttonMaterial6,
            }.ToImmutableArray();

            this.comboBoxCategory.DataSource = Enum.GetValues(typeof(CraftCategoryId));
            this.comboBoxCategory.SelectedIndexChanged += (_, _) =>
            {
                if (initailzing)
                    return;

                this.recipeViewModel!.CraftCategoryId = (CraftCategoryId)this.comboBoxCategory.SelectedItem;
            };

            this.buttonResultItem.Click += async (_, _) =>
            {
                var task = ItemSelectionBox.Show(this.buttonResultItem, this.recipeViewModel!.ResultItemID);

                await task;

                if (task.Result.result == DialogResult.OK)
                {
                    this.recipeViewModel.ResultItemID = task.Result.value!.id;
                    this.buttonResultItem.Text = task.Result.value!.Name;
                }
            };

            this.numericUpDownLevel.ValueChanged += (_, _) =>
            {
                if (initailzing)
                    return;

                this.recipeViewModel!.Level = (byte)this.numericUpDownLevel.Value;
            };

            for (int i = 0; i < this.materialbuttons.Length; i++)
            {
                var index = i;
                var button = this.materialbuttons[i];
                button.Click += async (_, _) =>
                {
                    var task = ItemSelectionBox.Show(button, this.recipeViewModel!.GetMaterial(index)?.id ?? 0);

                    await task;

                    if (task.Result.result == DialogResult.OK)
                    {
                        this.recipeViewModel.SetMaterial(index, task.Result.value!);
                        button.Text = task.Result.value!.Name;
                    }
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
                this.buttonResultItem.Text = ItemDataTable.Instance.Get(recipeViewModel.ResultItemID)?.Name ?? string.Empty;
                this.numericUpDownLevel.Value = recipeViewModel.Level;

                int i = 0;
                for (; i < recipeViewModel.MaterialCount; i++)
                    this.materialbuttons[i].Text = recipeViewModel.GetMaterial(i)?.Name ?? string.Empty;

                for (; i < 6; i++)
                    this.materialbuttons[i].Text = string.Empty;
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
