using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace Server
{
    class Program
    {
        
        static string input = "";

        static TcpListener listener;
        const int LIMIT = 5; //5 concurrent clients

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            string localComputerName = Dns.GetHostName();
            Console.WriteLine("DNS : " + localComputerName);
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress input in localIPs)
            {
                Console.WriteLine(input.ToString());
            }
            listener = TcpListener.Create(2055);
            listener.Start();

            Console.WriteLine("Server mounted, listening to port 2055");

            for (int i = 0; i < LIMIT; i++)
            {
                Thread t = new Thread(new ThreadStart(Service));
                t.Start();
            }
        }
        public static void Service()
        {

            string DirectoryPath = @"D:\";
            while (true)
            {
                Console.Title = "Centrall";
                Socket soc = listener.AcceptSocket();
                soc.NoDelay = true;
                Console.WriteLine("Connected: {0}", soc.RemoteEndPoint);
                try
                {
                    NetworkStream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s, Encoding.UTF8);
                    StreamWriter sw = new StreamWriter(s, Encoding.UTF8);
                    sw.AutoFlush = true; // enable automatic flushing


                    // start
                    string input;
                    sw.WriteLine("=====================");
                    sw.WriteLine("Hello in centrall!!!");
                    sw.WriteLine("Writed by Henri2h in 2016");
                    sw.WriteLine("=====================");
                    sw.WriteLine();
                    bool boocle = true;
                    while (boocle)
                    {
                        sw.WriteLine("<input/>");
                        input = sr.ReadLine();

                        if (input == "time") { sw.WriteLine(System.DateTime.Now); }
                        else if (input == "cls") { sw.WriteLine("This is sence to be cleared"); }
                        else if (input == "dir")
                        {
                            sw.WriteLine("");
                            sw.WriteLine("Ouverture du repertoire ....");
                            sw.WriteLine("Repertoire : D:\\");
                            sw.WriteLine("");

                            foreach (string file in Directory.EnumerateFileSystemEntries(DirectoryPath))
                            {


                                string isDirectory = "";
                                FileAttributes attr = File.GetAttributes(file);
                                if (attr.HasFlag(FileAttributes.Directory))
                                    isDirectory = "<dir>";
                                else
                                    isDirectory = "     ";
                                string outputStringFile = File.GetLastAccessTime(file) + "   " + isDirectory + "   " + Path.GetFileName(file);
                                sw.WriteLine(outputStringFile);
                            }
                            sw.WriteLine("");
                        }
                        else if (input == "cd")
                        {
                            sw.WriteLine("<Please enter direcory : />");
                            string inputDirectory = sr.ReadLine();
                            if (inputDirectory == "..")
                            {
                                try
                                {
                                    DirectoryPath = Directory.GetParent(DirectoryPath).ToString();
                                }
                                catch
                                {
                                    sw.WriteLine("cannot go out");
                                }
                            }
                            else
                            {
                                string preDirectoryPath = retrunPath(DirectoryPath, inputDirectory);
                                if (Directory.Exists(preDirectoryPath))
                                {
                                    DirectoryPath = preDirectoryPath;
                                }
                                else
                                {
                                    sw.WriteLine("This is not a directory");

                                }
                            }
                        }
                        else if (input == "type")
                        {
                            sw.WriteLine("<file : />");
                            string inputDirectory = retrunPath(DirectoryPath, sr.ReadLine());
                            sw.WriteLine(inputDirectory);
                            if (File.Exists(inputDirectory))
                            {
                                sw.WriteLine(File.ReadAllText(inputDirectory));
                            }
                            else
                            {
                                sw.WriteLine("This file doesn't exist");
                            }

                        }
                        else if (input == "chat")
                        {
                            while (true)
                            {
                                input = Console.ReadLine();
                                sw.WriteLine(input);

                            }

                        }
                        else if (input == "xml")
                        {
                            string xmlFileName = "D:\\Input.xml";
                            XmlDocument xmlDoc = new XmlDocument();
                            if (File.Exists(xmlFileName) == true)
                            {
                                xmlDoc.Load(xmlFileName);
                            }
                            else
                            {
                                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", null, null);
                                xmlDoc.AppendChild(xmlDeclaration);
                                XmlNode firstNode = xmlDoc.CreateNode(XmlNodeType.Element, "All", "");
                                xmlDoc.AppendChild(firstNode);
                            }


                            XmlElement xmlRoot = xmlDoc.DocumentElement;
                            XmlNode newNode = xmlDoc.CreateNode(XmlNodeType.Element, "Voc", "");
                            XmlNode nameNode = xmlDoc.CreateNode(XmlNodeType.Element, "Name", "");
                            sw.WriteLine("<#Name : />");
                            nameNode.InnerText = sr.ReadLine();
                            XmlNode versionNode = xmlDoc.CreateNode(XmlNodeType.Element, "Forname", "");
                            sw.WriteLine("<#Forname : />");
                            versionNode.InnerText = sr.ReadLine();
                            newNode.AppendChild(nameNode);
                            newNode.AppendChild(versionNode);
                            xmlRoot.AppendChild(newNode);

                            xmlDoc.Save(xmlFileName);

                        }
                        else if (input == "xml node")
                        {
                            string xmlFileName = "D:\\Input.xml";
                            XmlDocument xmlDoc = new XmlDocument();
                            if (File.Exists(xmlFileName) == true)
                            {
                                xmlDoc.Load(xmlFileName);
                                XmlNodeList nodeList;
                                XmlNode root = xmlDoc.DocumentElement;
                                nodeList = root.SelectNodes("//All");
                                foreach (XmlNode xmlNode in nodeList)
                                {
                                    XmlNode Name = xmlNode.SelectSingleNode("Name");
                                    XmlNode Forname = xmlNode.SelectSingleNode("Forname");
                                    sw.WriteLine("Name : " + Name.InnerText + " Forname : " + Forname.InnerText);
                                }
                            }
                        }
                        else if (input == "echo")
                        {
                            sw.WriteLine("<#input : />");
                            sw.WriteLine(sr.ReadLine());
                        }
                        else if (input == "shutdown")
                        {
                            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                            psi.CreateNoWindow = true;
                            psi.UseShellExecute = false;
                            Process.Start(psi);
                        }
                        else if(input == "sendFile")
                        {
                            sw.WriteLine("<recieve/>");
                            string srcPath = @"D:\map.txt";
                           
                            using (FileStream fs = new FileStream(srcPath, FileMode.Open, FileAccess.Read))
                            {
                                sw.WriteLine(fs.Length);
                                sw.WriteLine("map.txt");
                                long fileSize = fs.Length;
                                long sum = 0;   //sum here is the total of sent bytes.
                                int count = 0;
                                byte[] data = new byte[1024];  //8Kb buffer .. you might use a smaller size also.
                                while (sum < fileSize)
                                {
                                    count = fs.Read(data, 0, data.Length);
                                    s.Write(data, 0, count);
                                    sum += count;
                                }
                                s.Flush();
                            }
                        }
                        else if (input == "exit")
                        {
                            sw.WriteLine("<break/>");
                            break;
                        }
                        else { sw.WriteLine("please enter a valid input"); }
                    }
                    s.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("something went rong ...");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("Disconnected: {0}", soc.RemoteEndPoint);
                soc.Close();
            }
        }


        private static string retrunPath(string DirectoryPath, string inputDirectory)
        {
            if (DirectoryPath == Directory.GetDirectoryRoot(DirectoryPath))
            {
                return DirectoryPath + inputDirectory;
            }
            else
            {
                return DirectoryPath + "\\" + inputDirectory;
            }
        }
        static void exit()
        {
            Console.Title = "goodbye";
            Console.WriteLine("Press a key to exit");
        }

    }
}
