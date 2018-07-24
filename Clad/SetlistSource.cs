using System;
using System.Diagnostics;
using System.Collections.Generic;

using Clad.Models;
using Foundation;
using UIKit;

namespace Clad
{
    public class SetlistSource : UITableViewSource
    {
        IList<SetlistModel> _tableItems;
        string _cellIdentifier = "TableCell";

        public SetlistSource() { }

        public SetlistSource(IList<SetlistModel> items)
        {
            _tableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => _tableItems.Count;

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath) => true;

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath) => true;

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var item = _tableItems[sourceIndexPath.Row];
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
            _tableItems.Insert(insertAt, item);
            _tableItems.RemoveAt(deleteAt);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch(editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    _tableItems.RemoveAt(indexPath.Row);
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;
                case UITableViewCellEditingStyle.None:
                    Debug.Write("CommitEditingStyle:None called");
                    break;
            }
        }

        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath) => "Remove";

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath) => UITableViewCellEditingStyle.Delete;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            var item = _tableItems[indexPath.Row];

            cell.TextLabel.Text = $"{item.Key} - {item.BPM}";
            cell.TextLabel.MinimumFontSize = 24;
            cell.AccessibilityHint = $"{item.Key} at {item.BPM}";
            cell.AccessibilityLabel = $"{item.Key} at {item.BPM}";

            return cell;
        }
    }
}
