using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{

    public enum PointCloneColor { LightBlue, BloodRed, Gold }
    class PointClone : ICloneable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PointDescription desk = new PointDescription();
        public PointColor Color { get; set; }

        public PointClone(int xVal, int yVal, string petName)
        {
            Console.WriteLine(@"----------------Point X / Y -------------");
            X = xVal;
            Y = yVal;
            desk.petName = petName;
        }

        public PointClone()
        {}

        public override string ToString()
        {
            return String.Format("X = {0} Y = {1} \n Name = {2} \n ID = {3}", X, Y, desk.petName, desk.PointID);
        }

        public void DisplayStatus()
        {
            Console.WriteLine(@"X is: {0}, Y is: {1}", X, Y);
            Console.WriteLine(@" Point is: {0}", Color);
        }

        public object Clone()
        {
            //return new Point(this.X, this.Y);
            return this.MemberwiseClone();
        }
    }
}
