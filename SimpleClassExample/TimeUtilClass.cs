using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    static class TimeUtilClass
    {
        public static void PrintTime()
        {
                            Console.WriteLine(DateTime.Now.ToShortTimeString());


        }

        public static void PrintDate()
        {
            Console.WriteLine(DateTime.Now.ToShortDateString());


        }
    }
}
