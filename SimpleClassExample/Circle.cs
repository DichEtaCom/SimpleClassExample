using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Circle :Shape
    {
        public Circle()
        {
            
        }

        public Circle(string name) : base(name)
        {
            
        }

        public override void Draw()
        {
            Console.WriteLine(@"Drawning {0} the Circle ", PetName);
        }

    }
}
