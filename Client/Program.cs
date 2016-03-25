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
                                if (parameterName == "recieve")
                            {
                                long fileSize = long.Parse(sr.ReadLine());// your file size that you are going to receive it.
                                string name = sr.ReadLine();
                                string destPath = @"D:\" + name;
                                using (FileStream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                                {
                                    int count = 0;
                                    long sum = 0;   //sum here is the total of received bytes.
                                    byte[] data = new byte[1024 * 8];  //8Kb buffer .. you might use a smaller size also.
                                    while (sum < fileSize)
                                    {
                                        if (s.DataAvailable)
                                        {
                                            {

                                                count = s.Read(data, 0, data.Length);
                                                fs.Write(data, 0, count);
                                                sum += count;
                                            }
                                        }
                                    }
                                }
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
