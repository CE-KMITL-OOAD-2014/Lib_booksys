using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
namespace RobustnessTest
{
    class RobustnessTest
    {
        public static void Main(string[] args){
            double failcount = 0;
            File.WriteAllLines("Robusttest.txt",new List<string>{"Robustness test for paratabplus site."});
            ServicePointManager.DefaultConnectionLimit = 100000;
            //paratabplus ip = http://168.63.172.78
            Console.WriteLine("Robustness test for paratabplus site.");
            WebRequest request = WebRequest.Create("http://168.63.172.78");
            request.Timeout = 500;
            WebResponse response;
            DateTime d;
            string str;
            for (int i = 1; i <= 1000;i++)
            {
                try
                {
                    d = DateTime.Now;
                    response = request.GetResponse();
                    str = "#"+i + " Response time => " + ((d - DateTime.Now).Negate());
                    File.AppendAllLines("Robusttest.txt",new List<string>{str});
                    Console.WriteLine(str);
                }
                catch (Exception)
                {
                    str = "#"+i + " timeout";
                    File.AppendAllLines("Robusttest.txt",new List<string>{str});
                    Console.WriteLine(str);
                    failcount++;
                }
                request = WebRequest.Create("http://168.63.172.78");
                request.Timeout = 500;
            }
            str = "Fail time:" + failcount;
            Console.WriteLine(str);
            File.AppendAllLines("Robusttest.txt", new List<string> { str });
            str = "Down rate:" + ((failcount / 1000) * 100) + " percent.";
            File.AppendAllLines("Robusttest.txt", new List<string> {str});
            Console.WriteLine(str);
            Console.WriteLine("Test finished.Press anykey to exit.");
            Console.ReadKey(false);
        }
    }
}