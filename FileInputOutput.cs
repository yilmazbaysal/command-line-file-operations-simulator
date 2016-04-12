
using System;
using System.IO;

namespace CommandLine
{
    class FileInputOutput : InputOutput
    {
        StreamReader streamReader;
        StreamWriter streamWriter;

        public FileInputOutput(string inputFile, string outputFile)
        {
            try
            {
                streamReader = new StreamReader(inputFile);
                streamWriter = new StreamWriter(outputFile);
            }
            catch(Exception)
            {
                Console.WriteLine("Error : Input Output files could not opened.");
                System.Environment.Exit(1);
            }
        }

        public override string ReadLine()
        {
            return streamReader.ReadLine();
        }

        public override void WriteLine(string line)
        {
            Console.WriteLine(line);
            streamWriter.WriteLine(line);
        }

        public override void CloseFiles()
        {
            streamReader.Close();
            streamWriter.Close();
        }
    }
}
