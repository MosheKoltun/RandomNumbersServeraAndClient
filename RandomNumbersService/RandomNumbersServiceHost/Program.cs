using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RandomNumbersServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (System.ServiceModel.ServiceHost host = new
            System.ServiceModel.ServiceHost(typeof(RandomNumbersService.RandomNumbersService)))
            {
                host.Open();
                Console.WriteLine("Host started @ " + DateTime.Now.ToString());
                Console.ReadLine();
            }
        }
    }
}
