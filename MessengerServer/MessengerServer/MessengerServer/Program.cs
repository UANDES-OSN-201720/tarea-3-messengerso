using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    class Program
    {
        public Program()
        {
            Console.Title = "MessengerSO Server";
            Console.WriteLine("----- MesengerSO Server -----");
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.WriteLine("\nPress enter to close program.");
            Console.ReadLine();
        }
    }
}
