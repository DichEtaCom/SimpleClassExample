using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{

    public enum PointColor { LightBlue, BloodRed, Gold }
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PointColor Color { get; set; }

        public Point()
            : this(PointColor.BloodRed)
        {
            Console.WriteLine(@"----------------Point-------------");
        }

        public Point(int xVal, int yVal)
        {
            Console.WriteLine(@"----------------Point X / Y -------------");
            X = xVal;
            Y = yVal;
            Color = PointColor.Gold;
        }

        public Point(PointColor ptColor)
        {
            Console.WriteLine(@"----------------PointColor-------------");
            Color = ptColor;
        }

        public void DisplayStatus()
        {
            Console.WriteLine(@"X is: {0}, Y is: {1}", X, Y);
            Console.WriteLine(@" Point is: {0}", Color);
        }

        public object Clone()
        {
            return new Point(this.X, this.Y);
        }
    }
}
