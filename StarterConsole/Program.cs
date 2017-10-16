using System;

namespace StarterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Software starting");

            var sStart = new Cs.Software.Software();
            sStart.Run();


            Console.WriteLine("End!!");
            Console.ReadLine();

        }
    }
}
