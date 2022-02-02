using CSE681.JSON.DOMs;
using CSE681.JSON.Parse;
using CSE681.JSON.PrettyPrint;
using CSE681.JSON.Search;
using System.Windows;

namespace Project1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Value JsonDomTree { get; set; }
        private Searcher Searcher { get; set; } = new Searcher();
        private PrettyPrinter Printer { get; set; } = new PrettyPrinter();

        private void Button_Click_Parse_Text(object sender, RoutedEventArgs e)
        {
            Parser jsonParser = new Parser(TheText.Text);
            JsonDomTree = jsonParser.GetJsonValue();
        }

        private void Button_Click_Load_Text(object sender, RoutedEventArgs e)
        {
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

        private void Button_Click_Print_DOM(object sender, RoutedEventArgs e)
        {
            TheText.Text = Printer.PrettyPrintDOM(JsonDomTree);
        }

        private void Button_Click_Search_Json(object sender, RoutedEventArgs e)
        {
            Value jsonValue = Searcher
                 .SetDom(JsonDomTree)
                 .LookingFor(TheSearch.Text);

            if (jsonValue != null)
            {
                TheResults.Text = Printer.PrettyPrintDOM(jsonValue);
            }
        }
    }
}