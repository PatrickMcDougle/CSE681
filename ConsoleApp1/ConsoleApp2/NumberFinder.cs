using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class NumberFinder
    {
        public static int Find(List<int> list)
        {
            int maxValue = int.MinValue;

            foreach (int item in list)
            {
                maxValue = Math.Max(maxValue, item);
            }

            return maxValue;
        }
    }
}
