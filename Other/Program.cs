using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LuniaAssembly;
using LuniaAssembly.Packet;

namespace Other
{
    class Program
    {
        private static void Main(string[] args)
        {
            Random random = new Random();
            int max = 500;
            bool ready = false;
            string hype = "HYPE!";

            List<int> numbers = new List<int>();
            Task.Factory.StartNew(() =>
            {

                for (int i = 0; i < 100000000; i++)
                {
                    numbers.Add(random.Next(10, 40));
                }

                ready = true;

            });

            while (!ready)
            {
                //onsole.Title = i + "/" + max + " " + (int)((100f * i) / max) + "%";
                Console.Write(hype);
                Thread.Sleep(10);
            }

           




            Console.WriteLine("Durchschnitt: " + numbers.Average(x => x));


            Console.ReadKey();
        }
    }
}
