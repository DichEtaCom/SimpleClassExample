using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassExample
{
    class Employee
    {
        private string empName;
        private int empID;
        private float currPay;
        private int empAge;
        private string empSSN;

        public Employee() { }

        public Employee(string name, int id, float pay)
            : this(name, 0, id, pay, "")
        { }

        public Employee(string name, int age, int id, float pay, string ssn)
        {
            Name = name;
            ID = id;
            Pay = pay;
            empAge = age;
            empSSN = ssn;
        }

        public virtual void GiveBonus(float amount)
        {
            Pay += amount;
        }

        public virtual void DisplayStats()
        {
            Console.WriteLine(@"Name is: {0}", Name);
            Console.WriteLine(@"ID is: {0}", ID);
            Console.WriteLine(@"Age is: {0}", Age);
            Console.WriteLine(@"Pay is: {0}", Pay);
            Console.WriteLine(@"SSN is: {0}", empSSN);
        }

        public string GetName()
        {
            return empName;
        }

        public void SetName(string name)
        {
            if (name.Length > 15)
                Console.WriteLine(@"Error! Length name > 15 symbol");
            else
                empName = name;
        }

        public string Name
        {
            get { return empName; }
            set
            {
                if (value.Length > 15)
                    Console.WriteLine(@"Error! Length name > 15 symbol");
                else
                    empName = value;
            }
        }

        public int ID
        {
            get { return empID; }
            set { empID = value; }
        }

        public float Pay
        {
            get { return currPay; }
            set { currPay = value; }
        }

        public int Age
        {
            get { return empAge; }
            set { empAge = value; }
        }

        public string SocialSecuretyNumber
        {
            get { return empSSN; }
        }

    }
}
