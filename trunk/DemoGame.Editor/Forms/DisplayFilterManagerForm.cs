﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using NetGore.Editor;
using NetGore.Editor.WinForms;

namespace DemoGame.Editor
{
    public partial class DisplayFilterManagerForm : Form
    {
        MapDrawFilterHelperCollection _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayFilterManagerForm"/> class.
        /// </summary>
        public DisplayFilterManagerForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the <see cref="MapDrawFilterHelperCollection"/> to edit.
        /// </summary>
        public MapDrawFilterHelperCollection Collection
        {
            get { return _collection; }
            set
            {
                if (_collection == value)
                    return;

                // Clear out the old items
                lstItems.Items.Clear();

                if (_collection != null)
                {
                    // Remove the event hooks from the old collection
                    _collection.Added += _collection_Added;
                    _collection.Removed += _collection_Removed;
                    _collection.Renamed += _collection_Renamed;
                }

                _collection = value;

                lstItems.Collection = _collection;

                if (_collection != null)
                {
                    // Add the items for the new value
                    var toAdd = _collection.Filters.Select(x => x.Value).Cast<object>().ToArray();
                    lstItems.Items.AddRange(toAdd);

                    // Add the event hooks
                    _collection.Added += _collection_Added;
                    _collection.Removed += _collection_Removed;
                    _collection.Renamed += _collection_Renamed;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DesignMode)
            {
                base.OnClosing(e);
                return;
            }

            Collection = null;

            base.OnClosing(e);
        }

        /// <summary>
        /// Handles the corresponding event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MapDrawFilterHelperCollectionEventArgs"/> instance containing the event data.</param>
        void _collection_Added(MapDrawFilterHelperCollection sender, MapDrawFilterHelperCollectionEventArgs e)
        {
            lstItems.Items.Add(e.Filter);
        }

        /// <summary>
        /// Handles the corresponding event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MapDrawFilterHelperCollectionEventArgs"/> instance containing the event data.</param>
        void _collection_Removed(MapDrawFilterHelperCollection sender, MapDrawFilterHelperCollectionEventArgs e)
        {
            lstItems.Items.Remove(e.Filter);
        }

        /// <summary>
        /// Handles the corresponding event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MapDrawFilterHelperCollectionRenameEventArgs"/> instance containing the event data.</param>
        void _collection_Renamed(MapDrawFilterHelperCollection sender, MapDrawFilterHelperCollectionRenameEventArgs e)
        {
            var index = lstItems.Items.IndexOf(e.Filter);
            if (index >= 0)
                lstItems.RefreshItemAt(index);
        }

        /// <summary>
        /// Handles the Click event of the <see cref="btnAdd"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnAdd_Click(object sender, EventArgs e)
        {
            var c = Collection;
            if (c == null)
                return;

            var name = string.Empty;

            // Get the new filter's name
            do
            {
                const string msgInitial = "Enter the name of the new filter.";
                const string msgRetry = "A filter with that name already exists. Please try another name.";

                name = InputBox.Show("New filter name", string.IsNullOrEmpty(name) ? msgInitial : msgRetry, name);

                // User aborted
                if (string.IsNullOrEmpty(name))
                    return;
            }
            while (!string.IsNullOrEmpty(name) && c.TryGetFilter(name) != null);

            // Create and add the new filter
            var filter = new MapDrawFilterHelper();

            if (c.AddFilter(name, filter))
            {
                // Select the new filter
                lstItems.SelectedItem = filter;
            }
        }

        /// <summary>
        /// Handles the SelectedValueChanged event of the <see cref="lstItems"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void lstItems_SelectedValueChanged(object sender, EventArgs e)
        {
            pgItem.SelectedObject = lstItems.SelectedItem;
        }
    }
}