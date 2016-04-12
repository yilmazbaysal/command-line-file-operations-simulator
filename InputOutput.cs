
namespace CommandLine
{
    abstract class InputOutput
    {
        public abstract string ReadLine();
        public abstract void WriteLine(string line);

        public virtual void CloseFiles() { }
    }
}
