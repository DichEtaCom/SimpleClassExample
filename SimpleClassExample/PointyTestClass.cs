using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class PointyTestClass  : IPointy
    {
        public byte Points
        {
            get { throw new NotImplementedException(); }
        }
    }
}
