namespace RF5_CustomRecipeEditor
{
    public partial class MessageBox : Form
    {
        public MessageBox()
        {
            InitializeComponent();
        }

        public void Show(string message, IWin32Window? owner)
        {
            this.labelMessage.Text = message;
            this.Show(owner);
        }

        public void ShowDialog(string message)
        {
            this.labelMessage.Text = message;
            this.ShowDialog();
        }
    }
}
