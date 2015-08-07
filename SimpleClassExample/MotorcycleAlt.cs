using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class MotorcycleAlt
    {

        public int driverIntensity;
        public string driverName;
        public void PopAWheely()
        {
            Console.WriteLine(@"Yeeeeeeeeeeeee Haaaaaaaaaaaeewww!");
        }

        public void SetDriverName(string name)
        {
            driverName = name;
        }

        public MotorcycleAlt(int intersity = 0, string name = "")
        {
            if (intersity > 10)
                intersity = 10;
            driverIntensity = intersity;
            driverName = name;
        }
    }
}
