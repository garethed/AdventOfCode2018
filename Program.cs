using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    class Program
    {
        static void Main(string[] args)
        {
            var days = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Day).IsAssignableFrom(t) && t != typeof(Day));
            var day = (Day)days.OrderByDescending(d => int.Parse(d.Name.Substring(3))).First().GetConstructor(new Type[0] ).Invoke(new object[0]);

            try
            {
                Utils.WriteLine("**** DAY " + day.Index + "****", ConsoleColor.Cyan);

                Checkpoint();
                Utils.WriteLine("** TESTS **", ConsoleColor.Yellow);
                day.Test();
                Checkpoint();
                Utils.WriteLine("** SOLUTIONS **", ConsoleColor.Yellow);
                Utils.Write("Part 1: ", ConsoleColor.White);
                Utils.WriteLine(day.Part1(day.Input), ConsoleColor.Green);
                Checkpoint();
                Utils.Write("Part 2: ", ConsoleColor.White);
                Utils.WriteLine(day.Part2(day.Input), ConsoleColor.Green);
                Checkpoint();

            }
            catch (NotImplementedException)
            {
            }

            Utils.WriteLine("** FINISHED **", ConsoleColor.Cyan);
            Console.ReadLine();

        }

        static DateTime timer = DateTime.MaxValue;

        private static void Checkpoint()
        {
            if (timer != DateTime.MaxValue)
            {
                Utils.WriteLine("Completed in " + (DateTime.Now - timer).TotalSeconds.ToString("0.000s"), ConsoleColor.Gray);
            }
            timer = DateTime.Now;
        }
    }
}
