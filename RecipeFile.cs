using Newtonsoft.Json;

namespace RF5_CustomRecipeEditor
{
    public sealed class RecipeFile
    {
        private static RecipeFile instance;
        public static RecipeFile Instance
        {
            get
            {
                if (null == instance)
                    instance = new RecipeFile();
                return instance;
            }
        }

        List<Recipe> recipes;

        RecipeFile()
        {
            New();
        }

        public void New()
        {
            if (null == recipes)
                recipes = new List<Recipe>();
        }

        public void Load(string path)
        {
            if (File.Exists(path))
                recipes = JsonConvert.DeserializeObject<List<Recipe>>(File.ReadAllText(path));

            if (null == recipes)
                recipes = new List<Recipe>();
        }

        public void Save(string path)
        {
            recipes.AsParallel().ForAll(x =>
            {
                x.IngredientItemIDs = x.IngredientItemIDs
                    .Where(y => ItemDataTable.Instance.Contains(y))
                    .ToList();
            });

            File.WriteAllText(path, JsonConvert.SerializeObject(recipes, Formatting.Indented));
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