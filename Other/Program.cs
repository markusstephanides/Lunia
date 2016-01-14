using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuniaAssembly;
using LuniaAssembly.Packet;

namespace Other
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ID for LCAuthentication: " + Protocol.GetByType(typeof(LCAuthentication)));
            Console.WriteLine("ID for LCAuthenticationResponse: " + Protocol.GetByType(typeof(LCAuthenticationResponse)));

            Console.WriteLine("Type for LCAuthentication: " + Protocol.GetByID(1));
            Console.WriteLine("Type for LCAuthenticationResponse: " + Protocol.GetByID(2));

            Console.ReadKey();
        }
    }
}
