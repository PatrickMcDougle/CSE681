// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.JSON.DOMs;
using CSE681.JSON.Parse;
using CSE681.JSON.PrettyPrint;
using CSE681.JSON.Search;
using Microsoft.Win32;
using System.Windows;

namespace Project1
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Value FoundValue { get; set; }
        private Value JsonDomTree { get; set; }
        private PrettyPrinter Printer { get; set; } = new PrettyPrinter();
        private Searcher Searcher { get; set; } = new Searcher();
        private string searchValue { get; set; }

        private void Button_Click_Add_Json(object sender, RoutedEventArgs e)
        {
            Members members = new Members
            {
                Key = TheNewKey.Text,
                IsValid = true,
                Member = new String(TheNewValue.Text)
            };

            if (FoundValue != null)
            {
                if (FoundValue is Object obj)
                {
                    obj.Add(members);
                    TheResults.Text = Printer.PrettyPrintDOM(FoundValue);
                }
                else if (FoundValue is Array arr)
                {
                    // do soemthing.
                }
            }
        }

        private void Button_Click_Load_Text(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*",
                Title = "Open JSON File",
                InitialDirectory = @"C:\Users\Cland\OneDrive - Syracuse University\681 - CSE - Software Modeling & Analysis\Projects\01"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                TheText.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
            }

            //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");
        }

        private void Button_Click_Parse_Text(object sender, RoutedEventArgs e)
        {
            Parser jsonParser = new Parser(TheText.Text);
            JsonDomTree = jsonParser.GetJsonValue();
        }

        private void Button_Click_Print_DOM(object sender, RoutedEventArgs e)
        {
            TheText.Text = Printer.PrettyPrintDOM(JsonDomTree);
        }

        private void Button_Click_Search_Json(object sender, RoutedEventArgs e)
        {
            if (searchValue != TheSearch.Text)
            {
                // the search text has changed so reset FoundValue.
                FoundValue = null;
            }

            searchValue = TheSearch.Text;

            Value jsonValue = Searcher
                 .SetDom(JsonDomTree)
                 .SetAlreadyFound(FoundValue)
                 .LookingFor(searchValue);

            if (jsonValue != null)
            {
                FoundValue = jsonValue;
                TheResults.Text = Printer.PrettyPrintDOM(FoundValue);
            }
            else
            {
                FoundValue = null;
                TheResults.Text = "";
            }
        }

        private void LoadDefaultJson()
        {
            //DocumentObjectModel.Object myObject = new DocumentObjectModel.Object();

            //DocumentObjectModel.KeyValueSet keyValueSet = new DocumentObjectModel.KeyValueSet
            //{
            //    Key = "First Name",
            //    Value = new DocumentObjectModel.String("Patrick")
            //};
            //myObject.Add(keyValueSet);

            //keyValueSet = new DocumentObjectModel.KeyValueSet
            //{
            //    Key = "Last Name",
            //    Value = new DocumentObjectModel.String("McDougle")
            //};
            //myObject.Add(keyValueSet);

            //keyValueSet = new DocumentObjectModel.KeyValueSet
            //{
            //    Key = "Age",
            //    Value = new DocumentObjectModel.Number(46)
            //};
            //myObject.Add(keyValueSet);

            //keyValueSet = new DocumentObjectModel.KeyValueSet
            //{
            //    Key = "Height Feet",
            //    Value = new DocumentObjectModel.Number(6.25)
            //};
            //myObject.Add(keyValueSet);

            //keyValueSet = new DocumentObjectModel.KeyValueSet
            //{
            //    Key = "Married",
            //    Value = new DocumentObjectModel.Boolean(true)
            //};
            //myObject.Add(keyValueSet);
        }
    }
}