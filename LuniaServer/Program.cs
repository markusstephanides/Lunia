using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuniaServer
{
    class Program
    {
        static void Main(string[] args)
        {  
            StartupSettings settings = new StartupSettings();

            if (args.Contains("nogui")) settings.NoGUI = true;

            Server server = new Server(settings);

            Console.ReadKey();
        }
    }
}
