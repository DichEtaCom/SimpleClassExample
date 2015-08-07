using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleClassExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var m1 = new MotorcycleAlt();
            Console.WriteLine(@"Name {0} , Intersity {1}", m1.driverName, m1.driverIntensity);

            var m2 = new MotorcycleAlt(name: "Boss");
            Console.WriteLine(@"Name {0} , Intersity {1}", m2.driverName, m2.driverIntensity);
            var m3 = new MotorcycleAlt(7);
            Console.WriteLine(@"Name {0} , Intersity {1}", m3.driverName, m3.driverIntensity);


            var c = new Motorcycle(5);
            c.SetDriverName("Tiny");
            c.PopAWheely();
            Console.WriteLine(@"Rider name is {0} ", c.driverName);
            var cuck = new Car();
            cuck.PrintState();
            Console.WriteLine(@"--------------------------------------");
            Car mary = new Car("Mary");
            mary.PrintState();
            var myCar = new Car("Henry", 10);
            for (int i = 0; i <= 10; i++)
            {
                myCar.SpeedUp(5);
                myCar.PrintState();
            }
        }

        private void btnStaticDataandMembers_Click(object sender, RoutedEventArgs e)
        {

            TimeUtilClass.PrintDate();
            TimeUtilClass.PrintTime();



            var c1 = new SavingAccount(100);
            Console.WriteLine(@"Interest Rate is: {0}  / Balance is: {1}", SavingAccount.GetInterestRate(), c1.currBalance);

            var c2 = new SavingAccount(100.75);
            Console.WriteLine(@"Interest Rate is: {0}  / Balance is: {1}", SavingAccount.GetInterestRate(), c2.currBalance);
            var c3 = new SavingAccount(175.25);
            Console.WriteLine(@"Interest Rate is: {0}  / Balance is: {1}", SavingAccount.GetInterestRate(), c3.currBalance);
            SavingAccount.SetInterestRate(1.25);
            var c4 = new SavingAccount(775.75);
            Console.WriteLine(@"Interest Rate is: {0}  / Balance is: {1}", SavingAccount.GetInterestRate(), c4.currBalance);
            SavingAccount.InterestRat = 2.85;
            Console.WriteLine(@"Interest Rate is: {0}  / Balance is: {1}", SavingAccount.InterestRat, c4.currBalance);

        }

        private void btnRadio_Click(object sender, RoutedEventArgs e)
        {
            var viper = new Car();
            viper.TurnOnRadio(false);
        }

        private void btnEmployeeApp_Click(object sender, RoutedEventArgs e)
        {

            var newEmp = new Employee("Drozd", 178, 1500, 75, "10244");
            Console.WriteLine(@"SNN is: {0}", newEmp.SocialSecuretyNumber);

        }

        private void btnGarage_Click(object sender, RoutedEventArgs e)
        {
            var c = new Car();
            c.petName = "Opel";
            var g = new Garage();
            g.MyAuto = c;
            Console.WriteLine(@"Number of Car: {0}", g.NumberofGar);

            Console.WriteLine(@"PetName is: {0}", g.MyAuto.petName);
        }

        private void btnObjectInitializers_Click(object sender, RoutedEventArgs e)
        {
            var firstPoint = new Point();
            firstPoint.X = 10;
            firstPoint.Y = 11;
            firstPoint.DisplayStatus();
            Console.WriteLine(@"**********************************************************************************************************************");
            var anotherPoint = new Point(20, 22);
            anotherPoint.DisplayStatus();
            Console.WriteLine(@"**********************************************************************************************************************");
            var finalPoint = new Point { X = 30, Y = 33 };
            finalPoint.DisplayStatus();
            Console.WriteLine(@"**********************************************************************************************************************");

            Point goldPoint = new Point(PointColor.Gold) { X = 90, Y = 20 };
            goldPoint.DisplayStatus();

            Console.WriteLine(@"------------------------------------------------------------************************************---------------------------------------");
            var myRect = new Rectangle
            {
                TopLeft = new Point { X = 10, Y = 20 },
                BottonRight = new Point(PointColor.LightBlue) { X = 200, Y = 300 }
            };
            myRect.DisplayStatus();
            var myRect1 = new Rectangle
            {
                TopLeft = new Point { X = 10, Y = 20 },
                BottonRight = new Point(200, 300)
            };

            myRect1.DisplayStatus();
            Console.WriteLine(@"--------------------------------------------------------------------");
            PointClone p1 = new PointClone(50, 50, "PointCenter");
            p1.DisplayStatus();
            PointClone p2 = (PointClone)p1.Clone();
            p2.X = 0;
            Console.WriteLine(p1);
            Console.WriteLine(p2);
        }

        private void btnCarInheritance_Click(object sender, RoutedEventArgs e)
        {
            var myCar = new CarInheritance(80);
            myCar.Speed = 50;
            Console.WriteLine(@"My Car is goin : {0} MPH", myCar.Speed);
            myCar.Speed = 150;
            Console.WriteLine(@"My Car is goin : {0} MPH", myCar.Speed);
            var myCar1 = new CarInheritance();
            myCar1.Speed = 150;
            Console.WriteLine(@"My Car 1 is goin : {0} MPH", myCar1.Speed);
            var myVan = new MiniVan();
            myVan.Speed = 180;
            Console.WriteLine(@"My Van 1 is goin : {0} MPH", myVan.Speed);
        }

        private void btnManagerSalesNumbers_Click(object sender, RoutedEventArgs e)
        {

            var mn = new Manager("Chucky", 52, 92, 100000, "333-23-2322", 9000);
            mn.GiveBonus(375);
            mn.DisplayStats();

            var sp = new SalesPerson("Fran", 43, 93, 3000, "932-32-3232", 2);
            sp.GiveBonus(245);
            sp.DisplayStats();

        }

        private void btnShapess_Click(object sender, RoutedEventArgs e)
        {
            Shape[] myShapes = { new Hexagon(), new Circle(), new Hexagon("Mick"), new Circle("Beth"), new Hexagon("Linda") };

            foreach (Shape s in myShapes)
            {
                s.Draw();

                if (s is Hexagon)
                {
                    Hexagon ggg = (Hexagon)s;
                    Console.WriteLine(@"Points {0} the Hexagon ", ggg.Points);
                }
            }
        }


    }
}
