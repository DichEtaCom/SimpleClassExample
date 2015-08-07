using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Radio
    {
        public void Power(bool turnOn)
        {
            Console.WriteLine(@"Radio on: {0}", turnOn);
        }
    }
}
