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
        private int _demoStep = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private object FoundValue { get; set; }
        private object JsonDomTree { get; set; }
        private PrettyPrinter Printer { get; set; } = new PrettyPrinter();
        private Searcher Searcher { get; set; } = new Searcher();
        private string SearchValue { get; set; }

        private void Button_Click_DEMO(object sender, RoutedEventArgs e)
        {
            switch (_demoStep)
            {
                case 0:
                    TheDemoText.Text = "Load JSON -> Parse Text";
                    TheText.Text = "{\"menu\":{\"header\":\"SVG Viewer\",\"items\":[{\"id\":\"Open\"},{\"id\":\"OpenNew\",\"label\":\"Open New\"},{\"id\":\"About\",\"label\":\"About Adobe CVG Viewer...\"}]}}";
                    break;

                case 1:
                    TheDemoText.Text = "Parse Text -> Pretty Print";
                    Button_Click_Parse_Text(null, null);
                    break;

                case 2:
                    TheDemoText.Text = "Pretty Print -> Looking for ID (items)";
                    Button_Click_Print_DOM(null, null);
                    break;

                case 3:
                    TheDemoText.Text = "Looking for ID (items) -> Search for ID";
                    TheSearch.Text = "items";
                    break;

                case 4:
                    TheDemoText.Text = "Search for ID -> Load New Object";
                    Button_Click_Search_Json(null, null);
                    break;

                case 5:
                    TheDemoText.Text = "Load New Object -> Insert New Object";
                    TheNewJson.Text = "{\n    \"id\": \"New Object Added\"\n}";
                    break;

                case 6:
                    TheDemoText.Text = "Insert New Object -> Load Another Object";
                    Button_Click_Insert_Json(null, null);
                    break;

                default:
                    TheDemoText.Text = "Load Another Object?";
                    _demoStep = 3;
                    break;
            }

            _demoStep++;
        }

        private void Button_Click_Insert_Json(object sender, RoutedEventArgs e)
        {
            Parser jsonParser = new Parser(TheNewJson.Text);

            if (FoundValue != null)
            {
                if (FoundValue is Object obj)
                {
                    Members members = jsonParser.GetJsonMembers();
                    obj.Add(members);
                }
                else if (FoundValue is Array arr)
                {
                    object jsonValue = jsonParser.GetJsonValue();
                    arr.Add(jsonValue);
                    // do soemthing.
                }

                TheResults.Text = Printer.PrettyPrintDOM(FoundValue);
            }

            if (JsonDomTree != null)
            {
                TheText.Text = Printer.PrettyPrintDOM(JsonDomTree);
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

        private void Button_Click_Save_Text(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*",
                Title = "Open JSON File",
                InitialDirectory = @"C:\Users\Cland\OneDrive - Syracuse University\681 - CSE - Software Modeling & Analysis\Projects\01"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, TheText.Text);
            }
        }

        private void Button_Click_Search_Json(object sender, RoutedEventArgs e)
        {
            if (SearchValue != TheSearch.Text)
            {
                // the search text has changed so reset FoundValue.
                FoundValue = null;
            }

            SearchValue = TheSearch.Text;

            object jsonValue = Searcher
                 .SetDom(JsonDomTree)
                 .SetAlreadyFound(FoundValue)
                 .LookingFor(SearchValue, true);

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
    }
}