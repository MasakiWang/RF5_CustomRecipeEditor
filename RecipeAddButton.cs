namespace RF5_CustomRecipeEditor
{
    public partial class RecipeAddButton : UserControl
    {
        public event EventHandler ClickAdd
        {
            add => this.button1.Click += value;
            remove => this.button1.Click -= value;
        }

        public RecipeAddButton()
        {
            InitializeComponent();
        }
    }
}
