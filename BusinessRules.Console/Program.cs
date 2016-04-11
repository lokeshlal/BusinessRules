using BusinessRules.Client;
using System;

namespace BusinessRules.Console
{
    class Program
    {
        class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public float FloatField { get; set; } 
            public bool BooleanField { get; set; }
            public DateTime DateTimeField { get; set; }
            public string Address { get; set; }
        }

        static void Main(string[] args)
        {

            Person p = new Person()
            {
                Name = "Hello",
                Age = 26
            };

            BusinessRulesClient client = new BusinessRulesClient("http://localhost:61871/");
            p = client.ExecuteRule("PersonRule1", p);
            System.Console.WriteLine(string.Format("{0} - {1}", p.Name, p.Age));
            System.Console.ReadLine();
        }
    }
}
