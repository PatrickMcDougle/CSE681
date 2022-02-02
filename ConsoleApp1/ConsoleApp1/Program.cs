
using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<int> list = new List<int>
            {
                1,
                2,
                3,
                4,
                5,
                6,
                10,
                7,
                8,
                9
            };

            Console.WriteLine(NumberFinder.Find(list));
        }
    }
}
