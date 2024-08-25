using Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Example_Ribbon_Navigator.NavigationClass
{
    /// <summary>
    /// Manages the tabs in a KryptonNavigator control, handling tab creation, selection, and closing.
    /// </summary>
    public class TabManager : IDisposable
    {
        private readonly KryptonNavigator _navigator;
        private readonly Dictionary<string, KryptonPage> _tabs;
        private readonly Dictionary<string, Form> _forms;
        private readonly HashSet<string> _nonClosableTabs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabManager"/> class.
        /// </summary>
        /// <param name="navigator">The KryptonNavigator control to manage tabs for.</param>
        /// <exception cref="ArgumentNullException">Thrown when the navigator is null.</exception>
        public TabManager(KryptonNavigator navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _tabs = new Dictionary<string, KryptonPage>(StringComparer.OrdinalIgnoreCase); // Case-insensitive dictionary
            _forms = new Dictionary<string, Form>(StringComparer.OrdinalIgnoreCase); // Case-insensitive dictionary
            _nonClosableTabs = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Case-insensitive hash set

            // Configure the navigator's button settings
            _navigator.Button.ButtonDisplayLogic = ButtonDisplayLogic.None; // Hide default display logic
            _navigator.Button.CloseButtonAction = CloseButtonAction.RemovePage; // Set default close button action

            // Subscribe to the SelectedPageChanged event to update close button logic
            _navigator.SelectedPageChanged += Navigator_SelectedPageChanged;
        }

        /// <summary>
        /// Adds a new tab with the given form or selects an existing tab if already open.
        /// </summary>
        /// <param name="createForm">A function that creates the form to be added to the tab.</param>
        /// <exception cref="ArgumentNullException">Thrown when the createForm function is null.</exception>
        public void AddOrSelectTab(Func<Form> createForm)
        {
            if (createForm == null)
                throw new ArgumentNullException(nameof(createForm));

            Form form = createForm();
            string title = form.Text.Trim();

            if (IsOpenTab(title))
            {
                // If the tab is already open, select it and bring the form to the front
                _navigator.SelectedPage = _tabs[title];
                form.BringToFront();
            }
            else
            {
                // Otherwise, create a new tab
                CreateTab(form, title);
            }
        }

        /// <summary>
        /// Creates and adds a new tab with the specified form.
        /// </summary>
        /// <param name="form">The form to add to the new tab.</param>
        /// <param name="title">The title of the new tab.</param>
        private void CreateTab(Form form, string title)
        {
            KryptonPage newTab = new KryptonPage
            {
                Text = title
            };

            // Set form properties to integrate it into the tab
            form.TopLevel = false; // Form is not a top-level window
            form.FormBorderStyle = FormBorderStyle.None; // Remove borders
            form.Dock = DockStyle.Fill; // Fill the tab

            newTab.Controls.Add(form); // Add form to tab
            form.Show(); // Show the form

            _navigator.Pages.Add(newTab); // Add the tab to the navigator

            // Track the tab and form for later reference
            _tabs[title] = newTab;
            _forms[title] = form;

            _navigator.SelectedPage = newTab; // Select the new tab
        }

        /// <summary>
        /// Checks if a tab with the specified title is already open.
        /// </summary>
        /// <param name="tabName">The title of the tab to check.</param>
        /// <returns>True if the tab is open; otherwise, false.</returns>
        private bool IsOpenTab(string tabName)
        {
            // Check if any tab matches the title, ignoring case
            return _navigator.Pages.OfType<KryptonPage>().Any(p => p.Text.Equals(tabName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Prevents the specified tab from being closed.
        /// </summary>
        /// <param name="title">The title of the tab to prevent from closing.</param>
        public void PreventTabClose(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _nonClosableTabs.Add(title.Trim());
                UpdateCloseButtonAction();
            }
        }

        /// <summary>
        /// Allows the specified tab to be closed.
        /// </summary>
        /// <param name="title">The title of the tab to allow closing.</param>
        public void AllowTabClose(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _nonClosableTabs.Remove(title.Trim());
                UpdateCloseButtonAction();
            }
        }

        /// <summary>
        /// Updates the close button action based on the current tab's closability status.
        /// </summary>
        private void UpdateCloseButtonAction()
        {
            if (_navigator.SelectedPage != null)
            {
                string currentTitle = _navigator.SelectedPage.Text.Trim();
                // Set the close button action based on whether the current tab is non-closable
                _navigator.Button.CloseButtonAction = _nonClosableTabs.Contains(currentTitle)
                    ? CloseButtonAction.None
                    : CloseButtonAction.RemovePage;
            }
        }

        /// <summary>
        /// Handles the SelectedPageChanged event to update close button actions when the selected tab changes.
        /// </summary>
        private void Navigator_SelectedPageChanged(object sender, EventArgs e)
        {
            UpdateCloseButtonAction();
        }   

        /// <summary>
        /// Releases resources and unsubscribes from events to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe from events to prevent memory leaks
            _navigator.SelectedPageChanged -= Navigator_SelectedPageChanged;
        }
    }
}
