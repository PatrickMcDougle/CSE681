// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TokerApp
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Console.Write("\n  Testing semiExp Operations");
            Console.Write("\n ============================\n");

            IFactory factory = new Parser.JSON.Factory();
            IExpression expression = factory.CreateExpression(factory.CreateToker());

            string file = @"C:\Users\Cland\OneDrive - Syracuse University\681 - CSE - Software Modeling & Analysis\Projects\01\04 - one line.json";

            if (!expression.Open(file))
            {
                Console.Write("\n  Can't open file {0}", file);
                return;
            }

            while (expression.Get())
            {
                Debug.WriteLine(expression.Display());
            }
        }
    }
}