using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace BusinessRules.Console
{
    class Program
    {
        class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class ExecuteRuleDefinition
        {
            public object entity { get; set; }
            public string ruleName { get; set; }
        }
        static void Main(string[] args)
        {

            Person p = new Person()
            {
                Name = "Hello",
                Age = 26
            };

            ExecuteRuleDefinition executeRule = new ExecuteRuleDefinition()
            {
                entity = p,
                ruleName = "PersonRule1"
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:61871/");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var resp2 = client.PostAsync("api/async/executerule", new System.Net.Http.StringContent(JsonConvert.SerializeObject(executeRule), Encoding.UTF8, "application/json")).Result;
            resp2.EnsureSuccessStatusCode();
            var ss = resp2.Content.ReadAsStringAsync().Result;
            System.Console.WriteLine("--Output--");
            System.Console.WriteLine(ss);
            System.Console.ReadLine();
        }
    }
}
