using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class CarInheritance
    {
        public readonly int maxSpeed;
        private int currSpeed;


        public CarInheritance()
        {
            Console.WriteLine(@"--------CarInheritance-------");
            maxSpeed = 55;
            Console.WriteLine(maxSpeed);
        }

        public CarInheritance(int max)
        {
            Console.WriteLine(@"--------CarInheritance---max----");
            maxSpeed = max;
        }

        public int Speed
        {

            get { return currSpeed; }
            set
            {
                currSpeed = value;
                if (currSpeed > maxSpeed)
                    currSpeed = maxSpeed;
            }
        }

    }
}
