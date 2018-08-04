using System;
using System.Diagnostics;
using System.Collections.Generic;

using Clad.Models;
using Clad.Helpers;
using Foundation;
using UIKit;

namespace Clad
{
    /// <summary>
    /// Setlist source.
    /// </summary>
    [Register(nameof(SetlistSource))]
    public class SetlistSource : UITableViewSource
    {
        private IList<SetlistModel> _items;

        /// <summary>
        /// Gets or sets the items in the setlist
        /// </summary>
        /// <value>The items.</value>
        public IList<SetlistModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                LiteDbHelper.SaveCurrentSetlist(_items);
            }
        }
        string _cellIdentifier = "TableCell";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.SetlistSource"/> class.
        /// </summary>
        public SetlistSource() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.SetlistSource"/> class.
        /// </summary>
        /// <param name="items">Items.</param>
        public SetlistSource(IList<SetlistModel> items)
        {
            _items = items;
        }

        /// <summary>
        /// Rowses the in section.
        /// </summary>
        /// <returns>The in section.</returns>
        /// <param name="tableview">Tableview.</param>
        /// <param name="section">Section.</param>
        public override nint RowsInSection(UITableView tableview, nint section) => Items.Count;

        /// <summary>
        /// Cans the edit row.
        /// </summary>
        /// <returns><c>true</c>, if edit row was caned, <c>false</c> otherwise.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath) => true;

        /// <summary>
        /// Cans the move row.
        /// </summary>
        /// <returns><c>true</c>, if move row was caned, <c>false</c> otherwise.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath) => true;

        /// <summary>
        /// Moves the row.
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="sourceIndexPath">Source index path.</param>
        /// <param name="destinationIndexPath">Destination index path.</param>
        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var item = Items[sourceIndexPath.Row];
            var deleteAt = sourceIndexPath.Row;
            var insertAt = destinationIndexPath.Row;

            // are we inserting 
            if (destinationIndexPath.Row < sourceIndexPath.Row)
            {
                // add one to where we delete, because we're increasing the index by inserting
                deleteAt += 1;
            }
            else
            {
                // add one to where we insert, because we haven't deleted the original yet
                insertAt += 1;
            }
            Items.Insert(insertAt, item);
            Items.RemoveAt(deleteAt);

            LiteDbHelper.SaveCurrentSetlist(Items);
        }

        /// <summary>
        /// Commits the editing style.
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="editingStyle">Editing style.</param>
        /// <param name="indexPath">Index path.</param>
        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch(editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    Items.RemoveAt(indexPath.Row);
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    LiteDbHelper.SaveCurrentSetlist(Items);
                    break;
                case UITableViewCellEditingStyle.None:
                    Debug.Write("CommitEditingStyle:None called");
                    break;
            }
        }

        /// <summary>
        /// Titles for delete confirmation.
        /// </summary>
        /// <returns>The for delete confirmation.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath) => "Remove";

        /// <summary>
        /// Editings the style for row.
        /// </summary>
        /// <returns>The style for row.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath) => UITableViewCellEditingStyle.Delete;

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            var item = Items[indexPath.Row];

            cell.TextLabel.Text = $"{item.Key} - {item.BPM}";
            cell.TextLabel.MinimumFontSize = 24;
            cell.AccessibilityHint = $"{item.Key} at {item.BPM}";
            cell.AccessibilityLabel = $"{item.Key} at {item.BPM}";

            return cell;
        }
    }
}
