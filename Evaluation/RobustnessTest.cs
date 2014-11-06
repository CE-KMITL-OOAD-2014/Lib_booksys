using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
namespace RobustnessTest
{
    //This is code of trivial application for robustness evalution
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
        
            //Loop for 1000 times
            for (int i = 1; i <= 1000;i++)
            {
                /* use try-catch approach to send HTTPrequest to target and waiting for response in define time
                 * if response is not come in time,TimeoutException occur then count for what happen in failcount
                 * by one,otherwise send another request by create new everytime whether it receive response or not.
                 */ 
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
            
            //Calculate downrate by failcount/totalexperimenttime * 100
            str = "Down rate:" + ((failcount / 1000) * 100) + " percent.";
            File.AppendAllLines("Robusttest.txt", new List<string> {str});
            Console.WriteLine(str);
            Console.WriteLine("Test finished.Press anykey to exit.");
            Console.ReadKey(false);
        }
    }
}