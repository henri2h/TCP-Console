using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("192.168.0.14", 2055);

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

                try
                {

                    string inputText = sr.ReadLine();
                    MatchCollection matches = rgx.Matches(inputText);
                    if (matches.Count > 0)
                    {

                        MatchCollection matches_in = rgx_in.Matches(inputText);
                        string parameterName = matches_in[0].Value;

                        if (parameterName == "break")
                        {
                            break;
                        }
                        if (parameterName == "input")
                        {
                            Console.Write("input > ");
                            sw.WriteLine(Console.ReadLine());
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
