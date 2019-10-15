using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaltimoreCountyAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFilePath = @"C:\Users\nireddy\Downloads\Enigma\access.log";
            var outputFilePath = @"C:\Users\nireddy\Downloads\Enigma\report.csv";
            var IPAddrList = new List<string>();
            using (StreamReader file = new StreamReader(inputFilePath))
            {
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    //Ignoring Empty lines and lines that start with a non digit char which contains data that is not related to this report.
                    if (ln.Length < 0 || !(Regex.IsMatch(ln[0].ToString(), @"^\d")))
                    {
                        continue;
                    }

                    var lnSplit = ln.Split(' ');
                    //Checking for HTTP Port 80, Request Type to be GET, IP Doesn't start with 207.114 
                    if (lnSplit[7] == "80" && lnSplit[8] == "GET" && !lnSplit[2].ToString().StartsWith("207.114"))
                    {
                        IPAddrList.Add(lnSplit[2]);
                    }
                }
                file.Close();
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //This approach is a efficient solution that i can think of
            var Result = IPAddrList
                .GroupBy(x => x) //Group by to get count
                .Select(x => new { IPAddr = x.Key, Count = x.Count() }) //Building collection of type anonymous object type
                .OrderByDescending(x => x.Count) //Descending Order by Count of GET requests 
                .ThenByDescending(x => Int16.Parse(x.IPAddr.Split('.')[0])) //Then order by first IP octet in Descending
                .ThenByDescending(x => Int16.Parse(x.IPAddr.Split('.')[1])) //Then order by second IP octet in Descending
                .ThenByDescending(x => Int16.Parse(x.IPAddr.Split('.')[2])) //Then order by third IP octet in Descending
                .ThenByDescending(x => Int16.Parse(x.IPAddr.Split('.')[3])) //Then order by fourth IP octet in Descending
                .ToList();
            stopwatch.Stop();
            Console.WriteLine("Time with string split : " + stopwatch.ElapsedMilliseconds); //9ms

            String csv = String.Join(Environment.NewLine, Result.Select(d => $"{d.Count}, \"{d.IPAddr}\""));
            System.IO.File.WriteAllText(outputFilePath, csv);
        }
    }
}

