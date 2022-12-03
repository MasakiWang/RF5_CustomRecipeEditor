using Newtonsoft.Json;

namespace RF5_CustomRecipeEditor
{
    public sealed class RecipeFile
    {
        private static RecipeFile? instance = null;
        public static RecipeFile Instance
        {
            get
            {
                if (null == instance)
                    instance = new RecipeFile();
                return instance;
            }
        }

        List<Recipe> recipes = new List<Recipe>();

        RecipeFile()
        {
            New();
        }

        public void New()
        {
            recipes = new List<Recipe>();
        }

        public void Load(string path)
        {
            if (File.Exists(path))
                recipes = JsonConvert.DeserializeObject<List<Recipe>>(File.ReadAllText(path)) ?? new List<Recipe>();

            if (null == recipes)
                recipes = new List<Recipe>();
        }

        public void Save(string path)
        {
            var saveData = recipes
                .Select(x => x.Clone())
                .Where(Validate)
                .ToArray();

            if (0 == saveData.Length)
                return;

            saveData.AsParallel().ForAll(x =>
            {
                x.IngredientItemIDs = x.IngredientItemIDs
                    .Where(y => 0 != y && ItemDataTable.Instance.Contains(y))
                    .ToList();
            });

            File.WriteAllText(path, JsonConvert.SerializeObject(saveData, Formatting.Indented));
        }

        static bool Validate(Recipe recipe)
        {
            if (0 >= recipe.SkillLevel)
                return false;

            if (CraftCategoryId.EMPTY >= recipe.CraftCategoryId || CraftCategoryId.Max <= recipe.CraftCategoryId)
                return false;

            if (0 == recipe.ResultItemID || null == ItemDataTable.Instance.Get(recipe.ResultItemID))
                return false;

            if (recipe.IngredientItemIDs.All(x => x == 0) || recipe.IngredientItemIDs.All(x => null == ItemDataTable.Instance.Get(x)))
                return false;

            return true;
        }

        public IList<Recipe> GetRecipes()
        {
            return (IList<Recipe>)recipes ?? Array.Empty<Recipe>();
        }

        public void AddRecipe(Recipe recipe)
        {
            recipes.Add(recipe);
        }

        public void RemoveRecipe(Recipe recipe) 
        {
            recipes.Remove(recipe);
        }
    }
}