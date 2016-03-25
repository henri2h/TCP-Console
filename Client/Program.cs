using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Server IP : ");
            TcpClient client = new TcpClient(Console.ReadLine(), 2055);
            client.ReceiveTimeout = 500;
            NetworkStream s = client.GetStream();
            StreamReader sr = new StreamReader(s);
            StreamWriter sw = new StreamWriter(s);
            sw.AutoFlush = true;
            client.NoDelay = true;
            s.ReadTimeout = 1000;

            Console.WriteLine(client.Available.ToString());
            Console.WriteLine(s.CanRead.ToString());
            // find regex
            string pattern = @"^<(?:[^\>]*)+>";
            string pattern_in = @"(?:[^\<\/\>]+)";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            Regex rgx_in = new Regex(pattern_in, RegexOptions.IgnoreCase);
            // initialisation = ok
            while (true)
            {
                
                //string input_text = sr.ReadLine();
                sw.Flush();
                Console.WriteLine(client.Client.Connected.ToString());
                try
                {

                    string inputText = sr.ReadLine();
                    if (inputText != null)
                    {
                        MatchCollection matches = rgx.Matches(inputText);
                        if (matches.Count > 0)
                        {
                            MatchCollection matches_in = rgx_in.Matches(inputText);
                            string parameterName = matches_in[0].Value;

                            if (parameterName == "break")
                            {
                                break;
                            }
                            else
                            if (parameterName == "input")
                            {
                                Console.Write("input > ");
                                sw.WriteLine(Console.ReadLine());
                            }
                            else
                                if(parameterName == "recieve")
                            {
                                // to add
                            }
                            else
                            {
                                Console.Write(parameterName);
                                sw.WriteLine(Console.ReadLine());
                            }
                        }
                        else
                        {
                            Console.WriteLine(inputText);
                        }

                    }

                }
                catch (IOException)
                {
                    //sw.WriteLine(Console.ReadLine());
                }



                /*sw.WriteLine(Console.ReadLine());
                Console.WriteLine(sr.ReadLine());*/
            }

            s.Close();
            try
            { }
            finally
            {
                Console.WriteLine("client closed");
                client.Close();
                Console.ReadLine();
            }
        }
    }
}
