using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.IO;

namespace RF5_CustomRecipeEditor
{
    using TRecipeControl = RecipeControlv2;

    public partial class MainForm : Form
    {
        public static MainForm? Instance { get; private set; }

        string currentRecipeFilePath = string.Empty;
        RecipeAddButton recipeAddButton;
        List<TRecipeControl> recipeControls = new List<TRecipeControl>();
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
            this.saveAsToolStripMenuItem.Command = new RelayCommand(() => SaveFile(string.Empty));
            this.saveToolStripMenuItem.Command = new RelayCommand(() => SaveFile(currentRecipeFilePath));
            this.openToolStripMenuItem.Command = new RelayCommand(LoadFile);
            this.newToolStripMenuItem.Command = new RelayCommand(NewRecipe);

            this.Text = "Untitled recipe";

            Instance = this;
        }

        void ClearRecipe()
        {
            flowLayoutPanelRecipes.SuspendLayout();
            foreach (var c in flowLayoutPanelRecipes.Controls.OfType<TRecipeControl>())
                c.Visible = false;
            flowLayoutPanelRecipes.ResumeLayout();
        }

        void CreateRecipeControl()
        {
#if USE_V1
            Task.Run(async () =>
            {
                await Task.Delay(100);

                this.Invoke((Delegate)(() =>
                {
#endif
                    var newControl = new TRecipeControl();
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
#if USE_V1
                }));
            });

            messageBox.ShowDialog("Initialing comboxes...");
#endif
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

        void RemoveRecipe(TRecipeControl sender, Recipe args)
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

        void NewRecipe()
        {
            currentRecipeFilePath = string.Empty;
            this.Text = "Untitled recipe";
            RecipeFile.Instance.New();
            ClearRecipe();
        }

        void SaveFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                do
                {
                    using (var sfd = new SaveFileDialog())
                    {
                        sfd.FileName = Path.GetFileName(currentRecipeFilePath);
                        //sfd.InitialDirectory = Path.GetDirectoryName(currentRecipeFilePath);
                        sfd.RestoreDirectory = true;
                        sfd.Filter = "(*.json)|*.json";

                        if (DialogResult.OK != sfd.ShowDialog())
                            return;

                        this.Text = Path.GetFileName(currentRecipeFilePath = path = sfd.FileName);
                    }
                }
                while (string.IsNullOrEmpty(path));
            }

            RecipeFile.Instance.Save(path);
        }

        void LoadFile()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.FileName = Path.GetFileName(currentRecipeFilePath);
                //ofd.InitialDirectory = Path.GetDirectoryName(currentRecipeFilePath);
                ofd.RestoreDirectory = true;
                ofd.Filter = "(*.json)|*.json";

                if (DialogResult.OK != ofd.ShowDialog())
                    return;

                this.Text = Path.GetFileName(currentRecipeFilePath = ofd.FileName);
            }

            RecipeFile.Instance.Load(currentRecipeFilePath);
            ClearRecipe();
            AddRecipes(RecipeFile.Instance.GetRecipes());
        }

        private void combineRecipeJsonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.RestoreDirectory = true;
                ofd.Filter = "(*.json)|*.json";

                if (DialogResult.OK != ofd.ShowDialog())
                    return;

                if (1 >= ofd.FileNames.Length)
                    return;

                var combinedRecipes = ofd.FileNames
                    .SelectMany(x => JsonConvert.DeserializeObject<List<Recipe>>(File.ReadAllText(x)) ?? new List<Recipe>())
                    .OrderBy(x => x.ResultItemID)
                    .ToArray();

                using (var sfd = new SaveFileDialog())
                {
                    sfd.RestoreDirectory = true;
                    sfd.Filter = "(*.json)|*.json";

                    if (DialogResult.OK != sfd.ShowDialog()) 
                        return;

                    var jsonString = JsonConvert.SerializeObject(combinedRecipes);
                    File.WriteAllText(sfd.FileName, jsonString);
                }
            }
        }
    }
}