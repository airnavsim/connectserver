using System;

namespace StarterConsole
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var debug = new Cs.Debug();
            
            Cs.Settings.DebugToConsole = true;

            
            debug.Info("Software starting");
            var sStart = new Cs.Software.Software();
            sStart.Debug = debug;
            sStart.Run();


            debug.Info("End!!");
            Console.ReadLine();

        }
    }
}
