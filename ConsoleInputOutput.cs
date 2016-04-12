
using System;

namespace CommandLine
{
    class ConsoleInputOutput : InputOutput
    {
        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
