using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Triangles : Shape, IPointy
    {
        public Triangles()
        {
            
        }

        public Triangles(string name): base(name)
        {
            
        }

        public override void Draw()
        {
            Console.WriteLine(@"Drawning {0} the Triangles ", PetName);
        }

        public byte Points
        {
            get { return 3; }
        }
    }
}
