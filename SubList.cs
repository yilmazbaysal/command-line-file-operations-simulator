
namespace CommandLine
{
    class SubList : Node
    {
        private Node subList;

        public SubList(string name) : base()
        {
            subList = new Directory(name);
        }

        public override Node GetSubList()
        {
            return subList;
        }
        public override void SetSubList(Node subList)
        {
            this.subList = subList;
        }

        public override string GetName()
        {
            return subList.GetName();
        }

        public override string ToString(int count)
        {
            string str = "";

            Node temp = subList;

            while(temp != null)
            {
                if (temp.GetType() == typeof(SubList))
                {
                    str += temp.ToString(count);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        str += " |   ";
                    }

                    str += temp.ToString();
                }

                if (temp.GetType() == typeof(Directory))
                {
                    count++;
                }

                temp = temp.GetNext();
            }

            return str;
        }
    }
}
