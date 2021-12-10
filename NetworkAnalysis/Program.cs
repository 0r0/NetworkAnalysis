using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
namespace NetworkAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataPath = "D:\\Result.CSV";
            if (!File.Exists(dataPath))
            {
                InsertEntry(dataPath, "sourceIp", "destinationIp", "sourcePort", "destinationPort", "protocol", "duration", "sentByte", "receivebyte");
            }
            else
            {
                Console.WriteLine("File exist");
            }
            long sentByte ;
            long receivedByte;
            var startTime = DateTime.Now;
            
            while (DateTime.Now - startTime < TimeSpan.FromMinutes(1))
            {
                sentByte = 0;
                receivedByte = 0;
            WebRequest request = WebRequest.Create("https://docs.microsoft.com");

            //check network Exist
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            Stopwatch sw = Stopwatch.StartNew();
            foreach (NetworkInterface i in networkInterfaces)
            {
                sentByte += i.GetIPv4Statistics().BytesSent;
                receivedByte += i.GetIPv4Statistics().BytesReceived;
                Console.WriteLine("sent byte:{0}-{1}", i.GetIPv4Statistics().BytesSent,sentByte);
                Console.WriteLine(" receive byte:{0}-{1}", i.GetIPv4Statistics().BytesReceived,receivedByte);
                // Console.WriteLine("IP Address:{0}", i.GetIPv4Statistics().);
            }
            
            WebResponse response = request.GetResponse();
            sw.Stop();
            Console.WriteLine("duration time:" + sw.Elapsed.TotalMilliseconds);
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            string HostName = Dns.GetHostName();

            Console.WriteLine(HostName);
            Console.WriteLine("IP:" + Dns.GetHostByName(HostName).AddressList[0].ToString());

                var serverUrl = "https://docs.microsoft.com/";
                Uri myUri = new Uri(serverUrl);
                var destinationIp = Dns.GetHostAddresses(myUri.Host)[0];
                Console.WriteLine("destination ip" + destinationIp);
                Console.WriteLine("Host:" + myUri.Host);
                Console.WriteLine("port:" + myUri.Port);
                Console.WriteLine("protocol:" + myUri.Scheme);

                InsertEntry(dataPath,
                    Dns.GetHostByName(HostName).AddressList[0].ToString(), destinationIp.ToString(),
                    "", myUri.Port.ToString(), myUri.Scheme, 
                    sw.Elapsed.TotalMilliseconds.ToString(), sentByte.ToString(), receivedByte.ToString());
            }
           


        }

        public static void InsertEntry(string dataPath,string sourceIp,string destinationIp,
            string sourcePort,string destinationPort,string protocol,string
            duration,string sentByte,string receivebyte)
        {

            try
             {
                StringBuilder strLine = new StringBuilder();
                
                string line = sourceIp + "," + destinationIp + "," + sourcePort + "," + destinationPort + "," + sentByte + "," + receivebyte + "," + protocol + "," + duration;
                strLine.AppendLine(line);
                File.AppendAllText(dataPath, strLine.ToString());
                return;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }
    }
}
