using Example_Ribbon_Navigator.NavigationClass;
using Krypton.Toolkit;
using System;
using System.Windows.Forms;

namespace Example_Ribbon_Navigator
{
    public partial class RibbonForm : KryptonForm
    {
        private TabManager _tabManager;  // Instance of TabManager to manage tabs

        public RibbonForm()
        {
            InitializeComponent();
            _tabManager = new TabManager(kryptonNavigator1);

            // Initialize tab closure settings if needed
            // Example to prevent closing a specific tab
            _tabManager.PreventTabClose("Home Form");
        }

        private void btnForm1_Click(object sender, EventArgs e)
        {
            // Use TabManager to add or select the tab, creating a new Form1 instance
            _tabManager.AddOrSelectTab(() =>
            {
                var form1 = new Form1
                {
                    Text = "Form 1" // Set the title of the form
                };
                return form1;
            });
        }

        private void btnForm2_Click(object sender, EventArgs e)
        {
            // Use TabManager to add or select the tab, creating a new Form2 instance
            _tabManager.AddOrSelectTab(() => new Form2
            {
                Text = "Form 2" // Set the title of the form if needed
            });
        }

        private void btnHome3_Click(object sender, EventArgs e)
        {
            // Use TabManager to add or select the tab, creating a new Form3 instance
            _tabManager.AddOrSelectTab(() => new Form3
            {
                Text = "Form 3" // Set the title of the form if needed
            });
        }

        // Dispose TabManager when the form is closed
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _tabManager.Dispose();
            base.OnFormClosed(e);
        }
    }
}
