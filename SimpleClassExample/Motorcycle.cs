using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Motorcycle
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

        public Motorcycle()
        {
                            Console.WriteLine(@"In default ctor");
        }

        public Motorcycle(int intensity) : this(intensity, "")
        {
            Console.WriteLine(@"In default ctor an int");
        }

        public Motorcycle(string name): this (0, name)
        {
            Console.WriteLine(@"In default ctor an string");
        }

        public Motorcycle(int intersity, string name)
        {
            Console.WriteLine(@"In master ctor");
            if (intersity > 10)
                intersity = 10;
            driverIntensity = intersity;
            driverName = name;
        }
    }
}
