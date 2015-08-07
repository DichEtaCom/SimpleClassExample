using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Garage
    {

        public int NumberofGar { get; set; }
        public Car MyAuto { get; set; }


        public Garage()
        {
            MyAuto = new Car();
            NumberofGar = 1;
        }

        public Garage(Car car, int number)
        {
            MyAuto = car;
            NumberofGar = number;
        }










    }
}
