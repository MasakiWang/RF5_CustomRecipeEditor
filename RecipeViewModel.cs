namespace RF5_CustomRecipeEditor
{
    public sealed class RecipeViewModel
    {
        private Recipe recipe;

        public RecipeViewModel(Recipe recipe)
        {
            this.recipe = recipe;
        }

        public CraftCategoryId CraftCategoryId
        {
            get => recipe.CraftCategoryId;
            set => recipe.CraftCategoryId = value;
        }

        public byte Level
        {
            get => recipe.SkillLevel;
            set => recipe.SkillLevel = value;
        }

        public ItemDataRow GetMaterial(int index)
        {
            if (6 > index || 0 > index)
                throw new IndexOutOfRangeException();

            if (index >= recipe.IngredientItemIDs.Count)
                return ItemDataTable.Instance.Get(0);

            return ItemDataTable.Instance.Get(recipe.IngredientItemIDs[index]);
        }

        public void SetMaterial(int index, ItemDataRow itemData)
        {
            if (6 > index || 0 > index)
                throw new IndexOutOfRangeException();

            while (recipe.IngredientItemIDs.Count > index)
                recipe.IngredientItemIDs.Add(0);

            recipe.IngredientItemIDs[index] = itemData.id;
        }
    }
}