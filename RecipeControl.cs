using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RF5_CustomRecipeEditor
{
    public partial class RecipeControl : UserControl
    {
        RecipeViewModel recipeViewModel;

        public RecipeControl(RecipeViewModel recipeViewModel)
        {
            this.recipeViewModel = recipeViewModel;

            InitializeComponent();
        }
    }
}
