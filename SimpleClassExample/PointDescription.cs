using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class PointDescription
    {
        public string petName { get; set; }
        public Guid PointID { get; set; }
        public PointDescription()
        {
            petName = "No-Name";
            PointID = Guid.NewGuid();
        }
    }
}
