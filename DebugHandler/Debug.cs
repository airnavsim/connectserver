using System;
using System.Collections.Generic;
using System.Text;

namespace Cs
{
    public class Debug
    {
        public void Message(string message)
        {
            if (Settings.DebugToConsole)
            {
                Console.WriteLine(message);
            }
        }
    }
}
