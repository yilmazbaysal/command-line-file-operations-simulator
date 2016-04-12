
namespace CommandLine
{
    class File : AtomicElement
    {
        public File(string name) : base()
        {
            this.name = name;
        }

        public override string ToString()
        {
            return " |--- " + name + "\n";
        }
    }
}
