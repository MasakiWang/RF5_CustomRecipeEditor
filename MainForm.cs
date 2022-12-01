using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;

namespace RF5_CustomRecipeEditor
{
    public partial class MainForm : Form
    {
        List<Recipe> currentJsonRecipe;

        public MainForm()
        {
            InitializeComponent();
            InitAppConfig();
            InitComboBoxResultItemData();
        }

        void InitAppConfig()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(RF5_CustomRecipeEditor)) + $"{nameof(AppConfig)}.json";

            if (File.Exists(path))
                AppConfig.Instance = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(path));
            
            if (null == AppConfig.Instance)
                AppConfig.Instance = new AppConfig();

            while (string.IsNullOrWhiteSpace(AppConfig.Instance.Workspace) || !Directory.Exists(AppConfig.Instance.Workspace))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result;
                    do
                    {
                        result = fbd.ShowDialog();
                    } while (result != DialogResult.OK);

                    AppConfig.Instance.Workspace = fbd.SelectedPath;

                    File.WriteAllText(path, JsonConvert.SerializeObject(AppConfig.Instance, Formatting.Indented));
                }
            }
        }

        private void InitComboBoxResultItemData()
        {
            comboBoxResultItemData.DataSource = ItemDataTable.Instance.Items;
        }

        private void comboBoxResultItemData_SelectionChangeCommitted(object sender, EventArgs e)
        {
            flowLayoutPanelRecipes.Controls.Clear();

            var selectItem = (comboBoxResultItemData.SelectedItem as ItemDataRow);

            if (0 == selectItem?.id)
                return;

            var recipes = RecipeDatabase.Instance.GetRecipes(selectItem.id);
            foreach (var recipe in recipes)
                flowLayoutPanelRecipes.Controls.Add(new RecipeControl(new RecipeViewModel(recipe)));

            {
                var button = new RecipeAddButton();
                button.ClickAdd += (_, _) =>
                {
                    var button = flowLayoutPanelRecipes.Controls[flowLayoutPanelRecipes.Controls.Count - 1];
                    flowLayoutPanelRecipes.Controls.RemoveAt(flowLayoutPanelRecipes.Controls.Count - 1);
                    flowLayoutPanelRecipes.Controls.Add(new RecipeControl(new RecipeViewModel(new Recipe() { ResultItemID = selectItem.id })));
                    flowLayoutPanelRecipes.Controls.Add(button);
                };
                flowLayoutPanelRecipes.Controls.Add(button);
            }
        }
    }
}