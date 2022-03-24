// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.Windows.Controls;

namespace CSE681.Project4.GUI.Login
{
    /// <summary>Interaction logic for View.xaml</summary>
    public partial class View : DockPanel
    {
        public View(ViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}