 using System.ComponentModel;
using System.Windows.Forms;

namespace RF5_CustomRecipeEditor
{
    public partial class MainForm : Form
    {
        string currentRecipeFilePath = string.Empty;
        RecipeAddButton recipeAddButton;
        List<RecipeControl> recipeControls = new List<RecipeControl>();
        MessageBox messageBox = new MessageBox();

        public MainForm()
        {
            InitializeComponent();

            messageBox.Owner = this;

            recipeAddButton = new RecipeAddButton();
            recipeAddButton.ClickAdd += (_, _) =>
            {
                var newRecipe = new Recipe();

                RecipeFile.Instance.AddRecipe(newRecipe);

                AddRecipe(newRecipe);
            };
            flowLayoutPanelRecipes.Controls.Add(recipeAddButton);

            this.Text = "Untitled recipe";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((Keys.Control | Keys.S) == (keyData & (Keys.Control | Keys.S)))
            {
                if (keyData.HasFlag(Keys.Shift))
                    Save(string.Empty);
                else
                    Save(currentRecipeFilePath);
            }

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

        void CreateRecipeControl()
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);

                this.Invoke(() =>
                {
                    var newControl = new RecipeControl();
                    newControl.ClickDuplicate += (_, recipe) =>
                    {
                        var newRecipe = recipe.Clone();
                        RecipeFile.Instance.AddRecipe(newRecipe);
                        AddRecipe(newRecipe);
                    };
                    newControl.ClickRemove += (_, recipe) =>
                    {
                        RemoveRecipe(newControl, recipe);
                    };

                    recipeControls.Add(newControl);
                    flowLayoutPanelRecipes.Controls.Add(newControl);

                    messageBox.Hide();
                });
            });

            messageBox.ShowDialog("Initialing comboxes...");
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

            int i = recipeControls.FindIndex(x => false == x.Visible);
            i = (-1 == i) ? recipeControls.Count : i;

            for (int j = 0; j < recipes.Count; i++, j++)
            {
                if (0 == recipeControls.Count || i >= recipeControls.Count)
                    CreateRecipeControl();

                recipeControls[i].Visible = true;
                recipeControls[i].SetViewModel(new RecipeViewModel(recipes[j]));
            }

            for (; i < recipeControls.Count; i++)
                recipeControls[i].Visible = false;

            if (flowLayoutPanelRecipes.Controls[flowLayoutPanelRecipes.Controls.Count - 1] != recipeAddButton)
                flowLayoutPanelRecipes.Controls.Add(recipeAddButton);

            flowLayoutPanelRecipes.ResumeLayout();
        }

        void RemoveRecipe(RecipeControl sender, Recipe args)
        {
            flowLayoutPanelRecipes.SuspendLayout();

            sender.Visible = false;

            int index = recipeControls.IndexOf(sender);
            recipeControls.RemoveAt(index);
            recipeControls.Add(sender);

            index = flowLayoutPanelRecipes.Controls.IndexOf(sender);
            flowLayoutPanelRecipes.Controls.RemoveAt(index);
            flowLayoutPanelRecipes.Controls.RemoveAt(flowLayoutPanelRecipes.Controls.Count - 1);
            flowLayoutPanelRecipes.Controls.Add(sender);
            flowLayoutPanelRecipes.Controls.Add(recipeAddButton);

            RecipeFile.Instance.RemoveRecipe(args);

            flowLayoutPanelRecipes.ResumeLayout();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentRecipeFilePath = string.Empty;
            this.Text = "Untitled recipe";
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

                this.Text = currentRecipeFilePath = ofd.FileName;
            }

            RecipeFile.Instance.Load(currentRecipeFilePath);
            ClearRecipe();
            AddRecipes(RecipeFile.Instance.GetRecipes());
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(currentRecipeFilePath);
        }

        void Save(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                do
                {
                    using (var sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "(*.json)|*.json";

                        if (DialogResult.OK != sfd.ShowDialog())
                            return;

                        this.Text = currentRecipeFilePath = path = sfd.FileName;
                    }
                }
                while (string.IsNullOrEmpty(path));
            }

            RecipeFile.Instance.Save(path);
        }
    }
}