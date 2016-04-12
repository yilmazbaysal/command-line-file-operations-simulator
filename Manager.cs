
using System;

using System.Collections;

namespace CommandLine
{
    class Manager
    {
        GeneralizedList fileStructure;
        private InputOutput io;


        public Manager(string[] args)
        {
            fileStructure = new GeneralizedList();

            if (args.Length == 3)
            {
                io = new FileInputOutput(args[1], args[2]);
            }
            else
            {
                io = new ConsoleInputOutput();
            }

            CreateFileSystem(args[0]);

            OperateInputs();
        }

        private void OperateInputs()
        {
            string line = io.ReadLine();
            bool isSuccessfull = false;
            string operation = null;

            while(line != null && line != "exit")
            {  
                string[] words = line.Split(' ');

                if (words[0] == "ls")
                {
                    if(words.Length == 2)
                    {
                        WriteOperationTitle("List", words[1], null);
                        io.WriteLine(fileStructure.ToString(words[1]));
                    }
                    else
                    {
                        WriteOperationTitle("List", fileStructure.GetCurrentDirectoryUrl(), null);
                        io.WriteLine(fileStructure.ToString(""));
                    }
                }
                else if(words[0] == "cd")
                {
                    operation = "Change directory";
                    WriteOperationTitle(operation, words[1], null);

                    isSuccessfull = fileStructure.ChangeDirectory(words[1]);

                    if (!isSuccessfull)
                    {
                        io.WriteLine("Error : Change directory could not be executed.\n");
                    } 
                }
                else if(words[0] == "mv")
                {
                    if (words[1] == "-f")
                    {
                        operation = "Move file";
                    }
                    else
                    {
                        operation = "Move directory";
                    }

                    WriteOperationTitle(operation, words[2], words[3]);

                    isSuccessfull = Move(words[1], words[2], words[3]); 
                }
                else if (words[0] == "cp")
                {
                    if (words[1] == "-f")
                    {
                        operation = "Copy file";
                    }
                    else
                    {
                        operation = "Copy directory";
                    }

                    WriteOperationTitle(operation, words[2], words[3]);

                    isSuccessfull = Copy(words[1], words[2], words[3]);
                }
                else if(words[0] == "crtfl")
                {
                    operation = "Create file";
                    WriteOperationTitle(operation, words[1], null);

                    isSuccessfull = CreateFile(words[1]);

                    if(isSuccessfull == false)
                    {
                        io.WriteLine("Error : Create file could not be executed.\n");
                    }
                }
                else if (words[0] == "mkdir")
                {
                    operation = "Make directory";
                    WriteOperationTitle(operation, words[1], null);

                    isSuccessfull = MakeDirectory(words[1]);

                    if (!isSuccessfull)
                    {
                        io.WriteLine("Error : Make directory could not be executed.\n");
                    }
                }
                else if(words[0] == "rm")
                {
                    if (words[1] == "-f")
                    {
                        operation = "Remove file";
                    }
                    else
                    {
                        operation = "Remove directory";
                    }

                    WriteOperationTitle(operation, words[2], null);

                    isSuccessfull = fileStructure.Remove(words[1], words[2]);

                    if(isSuccessfull == false)
                    {
                        io.WriteLine("Error : " + operation + " could not be executed.\n");
                    }
                }

                if (isSuccessfull && words[0] != "ls")
                {
                    io.WriteLine(operation + " executed successfully.\n");
                }

                io.WriteLine("--------------------\n");

                line = io.ReadLine(); //Read a new line
            }

            io.CloseFiles();
        }

        public void CreateFileSystem(string initialSituation)
        {
            string directory = "";
            string temp = "";

            directory += initialSituation[0];

            for (int i = 1; i < initialSituation.Length; i++)
            {
                if (initialSituation[i] == '(')
                {
                    directory += '/';
                }
                else if (initialSituation[i] == ',' || i == initialSituation.Length - 1)
                {
                    fileStructure.AddDirectory(fileStructure.GetCurrentDirectory(), temp);

                    directory = directory.Substring(0, directory.LastIndexOf('/') + 1);
                }
                else if (initialSituation[i] == ')')
                {
                    if (initialSituation[i - 1] != ')')
                    {
                        temp = directory;
                    }

                    directory = directory.Substring(0, directory.LastIndexOf('/'));
                }
                else
                {
                    directory += initialSituation[i];
                    temp = directory;
                }
            }

            io.WriteLine("System initialized.\n");
        }

        public bool MakeDirectory(string url)
        {
            string name = null;

            for(int i = 1; i < url.Length; i++) // Check any directory named as '.'
            {
                if(url[i] != '/')
                {
                    name += url[i];
                }

                if (url[i] == '/' || i == url.Length - 1)
                {
                    if (name == ".")
                    {
                        return false;
                    }
                    name = null;
                }
            }

            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            if(url[0] == '.')
            {
                return fileStructure.AddDirectory(fileStructure.GetRoot(), url);
            }
            else
            {
                return fileStructure.AddDirectory(fileStructure.GetCurrentDirectory(), url);
            }
        }

        public bool CreateFile(string url)
        {
            string name = url.Substring(url.LastIndexOf('/') + 1);

            if (name == ".")
            {
                return false;
            }
            else if (url.Contains("/"))
            {
                if(fileStructure.GetNode(fileStructure.GetCurrentDirectory(), url.Remove(url.LastIndexOf('/'))) == null)
                {
                    return false;
                }
            }
            
            if(url[0] == '.')
            {
                return fileStructure.AddDirectory(fileStructure.GetRoot(), url);
            }
            else
            {
                return fileStructure.AddDirectory(fileStructure.GetCurrentDirectory(), url);
            }
            
        }

        public bool Move(string type, string source, string destination)
        {
            Node sourceNode = fileStructure.GetNode(fileStructure.GetCurrentDirectory(), source);
            Node destinationNode;

            if (sourceNode == null)
            {
                io.WriteLine("Error : Source directory to move the item does not exist.\n");
                return false;
            }
            else if (fileStructure.GetNode(fileStructure.GetCurrentDirectory(), destination + "/" + sourceNode.GetNext().GetName()) != null)
            {
                io.WriteLine("Error : A directory with the same name is already exists.\n");
                return false;
            }
            else if (type == "-d")
            {
                if (sourceNode.GetNext().GetType() == typeof(File))
                {
                    io.WriteLine("Error : The source item is not a directory.\n");
                    return false;
                }
                else if (destination == ".")
                {
                    destinationNode = fileStructure.GetRoot();
                }
                else
                {
                    destinationNode = fileStructure.GetNode(fileStructure.GetCurrentDirectory(), destination);

                    if (destinationNode != null && destinationNode.GetNext().GetType() == typeof(File))
                    {
                        io.WriteLine("Error : The destination to move is not a directory.\n");
                        return false;
                    }

                    // If destination does not exist, create it without error
                    if (!MakeDirectory(destination))
                    {
                        io.WriteLine("Error : The destination url is not a correct directory.\n");
                        return false;
                    }
                    destinationNode = fileStructure.GetNode(fileStructure.GetCurrentDirectory(), destination).GetNext();
                }

                if (destinationNode.GetType() == typeof(SubList))
                {
                    destinationNode = destinationNode.GetSubList();
                }

                Node toAdd = sourceNode.GetNext();

                fileStructure.Remove(type, source); //Remove the source

                toAdd.SetNext(null);
                fileStructure.AddToEnd(destinationNode, toAdd); //Add the source to end of the destination
            }
            else // Move File
            {
                if (sourceNode.GetNext().GetType() != typeof(File))
                {
                    io.WriteLine("Error : The source item is not a file.\n");
                    return false;
                }
                else if(fileStructure.GetNode(fileStructure.GetCurrentDirectory(), destination) != null)
                {
                    io.WriteLine("Error : A file with the same name is already exists.\n");
                    return false;
                }
                else
                {
                    bool isSuccessfull;

                    if (destination[0] == '.')
                    {
                        isSuccessfull = fileStructure.AddDirectory(fileStructure.GetRoot(), destination);
                    }
                    else
                    {
                        isSuccessfull = fileStructure.AddDirectory(fileStructure.GetCurrentDirectory(), destination);
                    }

                    if(isSuccessfull == false)
                    {
                        io.WriteLine("Error : The destination url is not a correct directory.\n");
                        return false;
                    }
                    else
                    {
                        fileStructure.Remove("-f", source); // Remove the file from source directory
                    }
                }  
            }

            return true; 
        }

        public bool Copy(string type, string source, string destination)
        {
            Node sourceNode = fileStructure.GetNode(fileStructure.GetCurrentDirectory(), source);
            bool isSuccessfull = false;

            if (sourceNode == null)
            {
                io.WriteLine("Error : Source directory to copy the item does not exists.\n");
                return false;
            }
            else if (type == "-f" && sourceNode.GetNext().GetType() != typeof(File))
            {
                io.WriteLine("Error : The source item is not a file.\n");
                return false;
            }
            else if (type == "-d" && sourceNode.GetNext().GetType() == typeof(File))
            {
                io.WriteLine("Error : The source item is not a directory.\n");
                return false;
            }
            else if (fileStructure.GetNode(fileStructure.GetCurrentDirectory(), destination + "/" + sourceNode.GetNext().GetName()) != null)
            {
                io.WriteLine("Error : A directory with the same name is already exists.\n");
                return false;
            }
            else if (type == "-f") //Copy File
            {
                if (fileStructure.GetNode(fileStructure.GetCurrentDirectory(), destination) != null)
                {
                    io.WriteLine("Error : A file with the same name is already exists.\n");
                    return false;
                }

                if (destination.Contains("/"))
                {
                    isSuccessfull = MakeDirectory(destination.Remove(destination.LastIndexOf('/')));
                }

                isSuccessfull = CreateFile(destination);
            }
            else
            {
                destination += "/" + sourceNode.GetNext().GetSubList().GetName();
                string directoriesToAdd = CopyDirectory(sourceNode.GetNext().GetSubList().GetNext(), destination, null);

                string[] directoryArray = directoriesToAdd.Split(' ');

                for(int i = 0; i < directoryArray.Length - 1; i++)
                {
                    if(directoryArray[i][0] == '.')
                    {
                        isSuccessfull = fileStructure.AddDirectory(fileStructure.GetRoot(), directoryArray[i]);
                    }
                    else
                    {
                        isSuccessfull = fileStructure.AddDirectory(fileStructure.GetCurrentDirectory(), directoryArray[i]);
                    }
                }
            }

            if (isSuccessfull == false)
            {
                io.WriteLine("Error : The destination url is not a correct directory.\n");
                return false;
            }
            else
            {
                return true;
            }
        }

        public string CopyDirectory(Node source, string destination, string directoriesToAdd)
        {
            if (source == null)
            {
                return directoriesToAdd;
            }

            string tempDestination;
            Node temp;

            if(source.GetType() == typeof(SubList))
            {
                tempDestination = destination + "/" + source.GetName();
                directoriesToAdd += tempDestination + "/" + " ";
                
                temp = source.GetSubList().GetNext();
            }
            else
            {
                directoriesToAdd += destination + "/" + source.GetName() + " ";

                tempDestination = destination;
                temp = source.GetNext();
            }

            directoriesToAdd = CopyDirectory(temp, tempDestination, directoriesToAdd); //Recursive call to the next element

            if(source.GetType() == typeof(SubList) && source.GetNext() != null)
            {
                directoriesToAdd = CopyDirectory(source.GetNext(), destination, directoriesToAdd);
            }

            return directoriesToAdd;
        }

        private void WriteOperationTitle(string operation, string source, string destination)
        {
            string toWrite = operation + " ";

            if (source != null && source[0] == '.')
            {
                toWrite += source;
            }
            else if (source != null)
            {
                toWrite += fileStructure.GetCurrentDirectoryUrl() + "/" + source;
            }

            if (destination != null && destination[0] == '.')
            {
                toWrite += " " + destination;
            }
            else if (destination != null)
            {
                toWrite += " " + fileStructure.GetCurrentDirectoryUrl() + "/" + destination;
            }

            io.WriteLine(toWrite);
        }
    }
}
