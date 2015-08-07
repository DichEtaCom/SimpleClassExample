using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Rectangle
    {
        private Point topLeft { get; set; }
        private Point bottonRight { get; set; }

        public Point TopLeft
        {
            get { return topLeft; }
            set { topLeft = value; }
        }

        public Point BottonRight
        {
            get { return bottonRight; }
            set { bottonRight = value; }
        }

        public void DisplayStatus()
        {
            Console.WriteLine(@" TopLeft: {0}, {1}, {2}  /  BottonRight: {3}, {4}, {5}",
                topLeft.X, topLeft.Y, topLeft.Color,
                bottonRight.X, bottonRight.Y, bottonRight.Color);
        }

    }
}
