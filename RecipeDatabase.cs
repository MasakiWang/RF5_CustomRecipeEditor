using Newtonsoft.Json;

namespace RF5_CustomRecipeEditor
{
    public sealed class RecipeDatabase
    {
        private static RecipeDatabase instance;
        public static RecipeDatabase Instance
        {
            get
            {
                if (null == instance)
                    instance = new RecipeDatabase();
                return instance;
            }
        }

        string savePath => Path.Combine(AppConfig.Instance.Workspace, "recipe.json");
        List<Recipe> recipes;
        IEnumerable<IGrouping<ushort, Recipe>> recipeGroups;

        RecipeDatabase()
        {
            if (File.Exists(savePath))
                recipes = JsonConvert.DeserializeObject<List<Recipe>>(File.ReadAllText(savePath));

            if (null == recipes)
                recipes = new List<Recipe>();

            recipeGroups = recipes.GroupBy(x => x.ResultItemID);
        }

        public IEnumerable<Recipe> GetRecipes(ushort resultItemID)
        {
            return recipeGroups.FirstOrDefault(g => g.Key == resultItemID) as IEnumerable<Recipe> ?? Array.Empty<Recipe>();
        }

        public void AddRecipe(Recipe recipe)
        {
            recipes.Add(recipe);
            recipeGroups = recipes.GroupBy(x => x.ResultItemID);
        }

        public void Save()
        {
            recipes.AsParallel().ForAll(x =>
            {
                x.IngredientItemIDs = x.IngredientItemIDs
                    .Where(y => ItemDataTable.Instance.Contains(y))
                    .ToList();
            });

            File.WriteAllText(savePath, JsonConvert.SerializeObject(recipes, Formatting.Indented));
        }
    }
}