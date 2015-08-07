using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Car
    {
        public string petName;
        public int currSpeed;
        private Radio myRadio = new Radio();
        public Car()
        {
            petName = "Chuck";
            currSpeed = 10;
        }

        public Car(string pn)
        {
            petName = pn;
        }

        public Car(string pn,int cs)
        {
            petName = pn;
            currSpeed = cs;
        }

        public void PrintState()
        {
            Console.WriteLine(@"{0} is going {1} MPF.", petName, currSpeed);
        }

        public void SpeedUp(int delta)
        {
            currSpeed += delta;
        }

        public void TurnOnRadio(bool onOff)
        {
            myRadio.Power(onOff);
        }

    }
}
