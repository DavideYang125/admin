using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = Task.Run(() => {
                Random rnd = new Random();
                long sum = 0;
                int n = 5000000;
                for (int ctr = 1; ctr <= n; ctr++)
                {
                    int number = rnd.Next(0, 101);
                    sum += number;
                    Console.WriteLine("ctr:    {0:N0}", ctr);
                }
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(150);
            if (!t.Wait(ts))
                Console.WriteLine("The timeout interval elapsed.");
        }
    }
}
