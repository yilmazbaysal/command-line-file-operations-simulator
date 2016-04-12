
namespace CommandLine
{
    class Node
    {
        private Node next;

        public Node()
        {
            next = null;
        }

        public Node GetNext()
        {
            return next;
        }

        public void SetNext(Node next)
        {
            this.next = next;
        }

        public virtual string GetName() { return null; }
        public virtual void SetName(string name) { }

        public virtual Node GetSubList() { return null; }
        public virtual void SetSubList(Node subList) { }

        public virtual string ToString(int count) { return null; }
    }
}
