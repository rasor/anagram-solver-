using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.services
{
    public class ConsoleLogger
    {
        public void ConsoleWriteLine(string stringToLog)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + stringToLog);
        }
    }
}
