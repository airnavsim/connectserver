using System;
using System.Collections.Generic;
using System.Text;

namespace Cs
{
    public class Debug
    {
        public void Info(string message)
        {
            this.Message(message);
        }
        public void Warning(String message)
        {
            this.Message(message);
        }
        public void Error(String message)
        {
            this.Message(message);
        }

        private void Message(string message)
        {
            if (Settings.DebugToConsole)
            {
                Console.WriteLine(message);
            }
        }
    }
}
