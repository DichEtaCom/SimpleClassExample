using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Hexagon : Shape, IPointy
    {
        public Hexagon() { }

        public Hexagon(string name)
            : base(name)
        {

        }

        public override void Draw()
        {
            Console.WriteLine(@"Drawning {0} the Hexagon ", PetName);
        }

        public byte Points
        {
            get { return 6; }
        }
    }
}
