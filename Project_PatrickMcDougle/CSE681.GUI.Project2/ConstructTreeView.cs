// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace CSE681.GUI.Project2
{
    internal class ConstructTreeView
    {
        private static readonly SolidColorBrush FOUND_COLOR = new SolidColorBrush(Color.FromRgb(220, 220, 0));

        private static readonly SolidColorBrush LIGHT_GREY_COLOR = new SolidColorBrush(Color.FromRgb(220, 220, 200));

        private static readonly SolidColorBrush WHITE_COLOR = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public TreeViewItem Build(object tree)
        {
            if (tree == null) return null;
            if (tree is JSON.DOMs.Array a) return Build(a);
            if (tree is JSON.DOMs.Boolean b) return Build(b);
            if (tree is JSON.DOMs.Members m) return Build(m);
            if (tree is JSON.DOMs.Number n) return Build(n);
            if (tree is JSON.DOMs.Object o) return Build(o);
            if (tree is JSON.DOMs.String s) return Build(s);

            return null;
        }

        public TreeViewItem Build(JSON.DOMs.Members members)
        {
            TreeViewItem treeViewItem = ConstructTreeViewItem($"\"{members.Key}\" :", members.UUID.ToString());
            treeViewItem.Items.Add(Build(members.Member));

            return treeViewItem;
        }

        public TreeViewItem Build(JSON.DOMs.Number number)
        {
            return ConstructTreeViewItem(number.IsWholeNumber ? $"{(int)number.TheValue}" : $"{number.TheValue}", number.UUID.ToString());
        }

        public TreeViewItem Build(JSON.DOMs.String str)
        {
            return ConstructTreeViewItem($"\"{str.TheValue}\"", str.UUID.ToString());
        }

        public TreeViewItem Build(JSON.DOMs.Boolean boolean)
        {
            return ConstructTreeViewItem(boolean.TheValue ? "True" : "False", boolean.UUID.ToString());
        }

        public TreeViewItem Build(JSON.DOMs.Object obj)
        {
            TreeViewItem treeViewItem = ConstructTreeViewItem("{ }", obj.UUID.ToString());
            treeViewItem.Background = LIGHT_GREY_COLOR;

            obj.TheValue
                .ToList()
                .ForEach(x => treeViewItem.Items.Add(Build(x)));

            return treeViewItem;
        }

        public TreeViewItem Build(JSON.DOMs.Array array)
        {
            TreeViewItem treeViewItem = ConstructTreeViewItem("[ ]", array.UUID.ToString());
            treeViewItem.Background = LIGHT_GREY_COLOR;

            array.TheValue
                .ToList()
                .ForEach(x => treeViewItem.Items.Add(Build(x)));

            return treeViewItem;
        }

        public bool SetFoundTreeViewItem(TreeViewItem treeViewItem, string uuid)
        {
            if (treeViewItem.Uid == uuid)
            {
                treeViewItem.Background = FOUND_COLOR;
                treeViewItem.IsExpanded = true;

                treeViewItem.Items
                    .Cast<TreeViewItem>()
                    .ToList()
                    .ForEach(x => x.IsExpanded = true);

                return true;
            }

            bool found = false;
            for (int i = 0; i < treeViewItem.Items.Count && !found; i++)
            {
                if (treeViewItem.Items[i] is TreeViewItem next)
                {
                    found = SetFoundTreeViewItem(next, uuid);
                    next.IsExpanded = found;
                }
            }
            treeViewItem.IsExpanded = found;
            return found;
        }

        public bool UnsetFoundTreeViewItem(TreeViewItem treeViewItem, string uuid)
        {
            if (treeViewItem.Uid == uuid)
            {
                treeViewItem.Background = WHITE_COLOR;
                treeViewItem.IsExpanded = false;

                treeViewItem.Items
                    .Cast<TreeViewItem>()
                    .ToList()
                    .ForEach(x => x.IsExpanded = false);

                return true;
            }

            bool found = false;
            for (int i = 0; i < treeViewItem.Items.Count && !found; i++)
            {
                if (treeViewItem.Items[i] is TreeViewItem next)
                {
                    found = UnsetFoundTreeViewItem(next, uuid);
                    if (found) next.IsExpanded = false;
                }
            }
            if (found) treeViewItem.IsExpanded = false;
            return found;
        }

        private TreeViewItem ConstructTreeViewItem(string header, string uuid)
        {
            return new TreeViewItem
            {
                Header = header,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Background = WHITE_COLOR,
                Uid = uuid
            };
        }
    }
}