
using System;

namespace CommandLine
{
    class GeneralizedList
    {
        private Node root;
        private Node currentDirectory;
        private string currentDirectoryUrl;

        public GeneralizedList()
        {
            root = new Directory(".");
            currentDirectory = root;
            currentDirectoryUrl = ".";
        }

        public Node GetRoot()
        {
            return root;
        }

        public string GetCurrentDirectoryUrl()
        {
            return currentDirectoryUrl;
        }

        public Node GetCurrentDirectory()
        {
            return currentDirectory;
        }

        public bool AddDirectory(Node temp, string directory)
        {
            if (directory.Length == 0)
            {
                return true;
            }
            else if(directory[0] == '.')
            {
                directory = directory.Substring(2);
            }

            string name = "";

            for(int i = 0; i < directory.Length; i++)
            {    
                 name += directory[i];

                 if(directory[i] == '/')
                 {
                     break;
                 }
            }
            directory = directory.Substring(name.Length);

            if (name.Contains("/"))
            {
                if (temp.GetNext() == null)
                {
                    temp.SetNext(new SubList(name.Remove(name.LastIndexOf('/'))));

                    return AddDirectory(temp.GetNext().GetSubList(), directory);
                }
                else if(temp.GetNext().GetType() != typeof(SubList))
                {
                    if(temp.GetNext().GetName() == name.Remove(name.LastIndexOf('/')))
                    {
                        return false;
                    }

                    return AddDirectory(temp.GetNext(), name + directory);
                }
                else if (temp.GetNext().GetSubList().GetName() == name.Remove(name.LastIndexOf('/')))
                {
                    return AddDirectory(temp.GetNext().GetSubList(), directory);
                }
                else
                {
                    return AddDirectory(temp.GetNext(), name + directory);
                }
            }
            else
            {
                bool isSuccessFull = AddToEnd(temp, new File(name));
                if ( !isSuccessFull )
                {
                    return false;
                }

                return AddDirectory(temp, directory);
            }
        }

        public Node GetNode(Node toGet, string url)
        {
            string name = "";

            if (url != "" && url[0] == '.')
            {
                url = url.Substring(1);
                toGet = root;
            }

            if (url == "")
            {
                return toGet;
            }
            else if (url[0] == '/')
            {
                url = url.Substring(1);
            }

            for (int i = 0; i < url.Length; i++)
            {
                if (url[i] == '/')
                {
                    break;
                }

                name += url[i];
            }
            url = url.Substring(name.Length);

            if(toGet.GetNext() == null)
            {
                return null; // If there is no such directory
            }
            else if(toGet.GetNext().GetType() == typeof(SubList) && toGet.GetNext().GetSubList().GetName() == name)
            {
                if (!url.Contains("/"))
                {
                    return toGet;
                }

                return GetNode(toGet.GetNext().GetSubList(), url);
            }
            else if(toGet.GetNext().GetName() == name)
            {
                if (!url.Contains("/"))
                {
                    return toGet;
                }

                return GetNode(toGet.GetNext(), url);
            }
            else
            {
                return GetNode(toGet.GetNext(), name + url);
            }
        }

        public bool ChangeDirectory(string url)
        {
            if (url == ".")
            {
                currentDirectory = root;
                currentDirectoryUrl = ".";
            }
            else
            {
                try
                {
                    if(GetNode(currentDirectory, url).GetNext().GetSubList().GetType() == typeof(File))
                    {
                        return false;
                    }

                    currentDirectory = GetNode(currentDirectory, url).GetNext().GetSubList();
                }
                catch(Exception)
                {
                    return false;
                }
                
                if(url[0] == '.')
                {
                    currentDirectoryUrl = url;
                }
                else
                {
                    currentDirectoryUrl = currentDirectoryUrl + "/" + url;
                }
            }

            return true;
        }

        public bool Remove(string type, string url) 
        {
            Node temp = GetNode(currentDirectory, url);

            if(temp == null)
            {
                return false;
            }
            else if(type == "-d" && url == ".")
            {
                return false;
            }
            else if (type == "-d" && temp.GetNext().GetType() == typeof(File))
            {
                return false;
            }
            else if(type == "-f" && temp.GetNext().GetType() != typeof(File))
            {
                return false;
            }
            else
            {
                temp.SetNext(temp.GetNext().GetNext());
                return true;
            }
        }

        public bool AddToEnd(Node list, Node toAdd)
        {
            Node temp = list;

            do
            {
                if (temp != list && temp.GetName() == toAdd.GetName())
                {
                    break;
                }
                else if (temp.GetNext() == null)
                {
                    temp.SetNext(toAdd);

                    return true; //Added succesfully
                }   

                temp = temp.GetNext();

            } while (temp != null);

            return false; //If an item has already axists with the same name
        }

        public string ToString(string url)
        {
            Node temp = GetNode(currentDirectory, url);

            if(temp == null)
            {
                return "Error : The directory to list does not exists.\n";
            }
            else if(url == ".")
            {
                temp = root;
            }
            else if(url != "")
            {
                if(temp.GetNext().GetType() == typeof(SubList))
                {
                    temp = temp.GetNext().GetSubList();
                }
                else
                {
                    return "Error : A file connot be listed.\n";
                } 
            }
            
            string str = "<" + temp.GetName() + ">\n";
            temp = temp.GetNext();

            while (temp != null)
            {
                if(temp.GetType() == typeof(SubList))
                {
                    str += temp.ToString(0);
                }
                else
                {
                    str += temp.ToString();
                }
                
                temp = temp.GetNext();
            }

            return str;
        }
    }
}
