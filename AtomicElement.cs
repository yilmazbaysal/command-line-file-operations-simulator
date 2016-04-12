
using CommandLine;

class AtomicElement : Node
{
    protected string name;

    public override string GetName()
    {
        return name;
    }
    public override void SetName(string name)
    {
        this.name = name;
    }
}
