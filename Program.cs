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
            var days = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Day).IsAssignableFrom(t) && t != typeof(Day)).OrderByDescending(d => int.Parse(d.Name.Substring(3)));

            if (args.Length > 0 && args[0].ToLower().Contains("all"))
            {
                foreach (var type in days.Reverse())
                {
                   if (!RunDay(type))
                    {
                        break;
                    }
                }
            }
            else if (args.Length > 0)
            {
                RunDay(days.Where(d => d.Name.Substring(3) == args[0]).First());
            }
            else
            {
                RunDay(days.First());
            }

            

            Utils.WriteLine("** FINISHED **", ConsoleColor.Cyan);
            Console.ReadLine();

        }

        static bool RunDay(Type dayType)
        {
            var day = (Day)dayType.GetConstructor(new Type[0]).Invoke(new object[0]);

            try
            {
                Utils.WriteLine("**** DAY " + day.Index + "****", ConsoleColor.Cyan);

                Checkpoint();
                Utils.WriteLine("** TESTS **", ConsoleColor.Yellow);
                var test = day.Test();
                Checkpoint();

                if (test)
                {
                    Utils.WriteLine("** SOLUTIONS **", ConsoleColor.Yellow);
                    Utils.Write("Part 1: ", ConsoleColor.White);
                    Utils.WriteLine(day.Part1(day.Input, day.Options), ConsoleColor.Magenta);
                    Checkpoint();
                    Utils.Write("Part 2: ", ConsoleColor.White);
                    Utils.WriteLine(day.Part2(day.Input, day.Options), ConsoleColor.Magenta);
                    Checkpoint();
                    return true;
                }

            }
            catch (NotImplementedException)
            {
            }

            return false;
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
