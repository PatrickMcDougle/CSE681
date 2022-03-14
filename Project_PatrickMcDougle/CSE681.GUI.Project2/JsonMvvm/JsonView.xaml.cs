// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.Windows;
using System.Windows.Input;

namespace CSE681.GUI.Project2
{
    /// <summary>Interaction logic for JsonView.xaml</summary>
    public partial class JsonView : Window
    {
        public JsonView()
        {
            InitializeComponent();

            DataContext = new JsonViewModel(new JsonModel());
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //txtEditor.Text = "";
        }
    }
}