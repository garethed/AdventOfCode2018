using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AdventOfCode2018
{
    class Day5 : Day
    {

        public override void Test()
        {
            Utils.Test(Part1, "dabAcCaCBAcCcaDA", "10" );
            Utils.Test(Part2, "dabAcCaCBAcCcaDA", "4");
        }

     
        public override string Part1(string input, dynamic options)
        {
            return Collapse(input, '@').ToString(); // any non letter for filter does nothing
                        
        }

        public override string Part2(string input, dynamic options)
        {
            var min = int.MaxValue;

            foreach (char c in "abcdefghjiklmnopqrstuvwxyz")
            {
                var result = Collapse(input, c);
                min = Math.Min(min, result);
            }
            return min.ToString();
        }        

        private int Collapse(string input, char filter)
        {
            Stack<char> result = new Stack<char>();

            foreach (char next in (string)input)
            {
                if (char.ToLower(next) == filter)
                {
                    continue;
                }

                if (result.Count > 0)
                {
                    char head = result.Peek();

                    if (char.ToLower(head) == char.ToLower(next) && head != next)
                    {
                        result.Pop();
                        continue;
                    }
                }
                result.Push(next);
            }

            return result.Count;
        }
    }

}
