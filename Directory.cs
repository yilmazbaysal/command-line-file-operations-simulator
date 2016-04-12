
namespace CommandLine
{
    class Directory : AtomicElement
    {
        public Directory(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return " |---<" + name + ">\n";
        }
    }
}
