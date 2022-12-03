namespace RF5_CustomRecipeEditor
{
    public sealed class RecipeViewModel
    {
        public readonly Recipe Recipe;

        public RecipeViewModel(Recipe recipe)
        {
            this.Recipe = recipe;
        }

        public ushort ResultItemID
        {
            get => Recipe.ResultItemID;
            set => Recipe.ResultItemID = value;
        }

        public CraftCategoryId CraftCategoryId
        {
            get => Recipe.CraftCategoryId;
            set => Recipe.CraftCategoryId = value;
        }

        public byte Level
        {
            get => Recipe.SkillLevel;
            set => Recipe.SkillLevel = value;
        }

        public int MaterialCount => Recipe.IngredientItemIDs.Count;

        public ItemDataRow? GetMaterial(int index)
        {
            if (6 <= index || 0 > index)
                throw new IndexOutOfRangeException();

            if (index >= Recipe.IngredientItemIDs.Count)
                return ItemDataTable.Instance.Get(0);

            return ItemDataTable.Instance.Get(Recipe.IngredientItemIDs[index]);
        }

        public void SetMaterial(int index, ItemDataRow itemData)
        {
            if (6 <= index || 0 > index)
                throw new IndexOutOfRangeException();

            while (index >= Recipe.IngredientItemIDs.Count)
                Recipe.IngredientItemIDs.Add(0);

            Recipe.IngredientItemIDs[index] = itemData.id;
        }
    }
}