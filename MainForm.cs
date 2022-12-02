using System.ComponentModel;

namespace RF5_CustomRecipeEditor
{
    public partial class MainForm : Form
    {
        string currentRecipeFilePath;
        RecipeAddButton recipeAddButton;
        List<RecipeControl> recipeControls = new List<RecipeControl>();

        public MainForm()
        {
            InitializeComponent();

            if (null == recipeAddButton)
            {
                recipeAddButton = new RecipeAddButton();
                recipeAddButton.ClickAdd += (_, _) =>
                {
                    var newRecipe = new Recipe();

                    RecipeFile.Instance.AddRecipe(newRecipe);

                    AddRecipe(newRecipe);
                };
            }
            flowLayoutPanelRecipes.Controls.Add(recipeAddButton);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
                Save();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        void ClearRecipe()
        {
            flowLayoutPanelRecipes.SuspendLayout();
            foreach (var c in flowLayoutPanelRecipes.Controls.OfType<RecipeControl>())
                c.Visible = false;
            flowLayoutPanelRecipes.ResumeLayout();
        }

        static Recipe[] tmpArray = new Recipe[1];
        void AddRecipe(Recipe recipe)
        {
            tmpArray[0] = recipe;

            AddRecipes(tmpArray);
        }

        void AddRecipes(IList<Recipe> recipes)
        {
            flowLayoutPanelRecipes.SuspendLayout();

            int i = 0;
            for (; i < recipes.Count; i++)
            {
                if (i >= recipeControls.Count)
                {
                    var newControl = new RecipeControl();

                    recipeControls.Add(newControl);
                    flowLayoutPanelRecipes.Controls.Add(newControl);
                }

                recipeControls[i].Visible = true;
                recipeControls[i].SetViewModel(new RecipeViewModel(recipes[i]));
            }

            for (; i < recipeControls.Count; i++)
                recipeControls[i].Visible = false;

            if (flowLayoutPanelRecipes.Controls[flowLayoutPanelRecipes.Controls.Count - 1] != recipeAddButton)
                flowLayoutPanelRecipes.Controls.Add(recipeAddButton);

            flowLayoutPanelRecipes.ResumeLayout();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentRecipeFilePath = string.Empty;
            RecipeFile.Instance.New();
            ClearRecipe();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "(*.json)|*.json";

                if (DialogResult.OK != ofd.ShowDialog())
                    return;

                currentRecipeFilePath = ofd.FileName;
            }

            RecipeFile.Instance.Load(currentRecipeFilePath);
            ClearRecipe();
            AddRecipes(RecipeFile.Instance.GetRecipes());
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        void Save()
        {
            if (string.IsNullOrEmpty(currentRecipeFilePath) || !File.Exists(currentRecipeFilePath))
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "(*.json)|*.json";

                    if (DialogResult.OK != sfd.ShowDialog())
                        return;

                    currentRecipeFilePath = sfd.FileName;
                }
            }

            RecipeFile.Instance.Save(currentRecipeFilePath);
        }
    }
}