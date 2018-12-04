using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    class Day1 : Day
    {
        public override void Test()
        {
            Utils.Test(Part1, new[] { "+1\n +1\n +1", "+1\n +1\n -2", "-1\n -2\n -3" }, new[] { "3", "0", "-6" });
            Utils.Test(Part2, new[] { "+1\n -1", " +3\n +3\n +4\n -2\n -4", "-6\n +3\n +8\n +5\n -6", "+7\n +7\n -2\n -7\n -4" }, new[] { "0", "10", "5", "14" });
        }

        public override string Part1(dynamic input)
        {
            int f = 0;
            foreach (string delta in Utils.splitLines(input))
            {
                f += int.Parse(delta);
            }

            return f.ToString();      
        }

        public override string Part2(dynamic input)
        {
            HashSet<int> found = new HashSet<int>();
            int f = 0;
            found.Add(f);

            foreach (var i in loop(input))
            {
                f += i;
                if (found.Contains(f))
                {
                    return f.ToString();
                }
                found.Add(f);
            }

            return null;
        }

        private IEnumerable<int> loop(string input)
        {
            var items = Utils.splitLines(input);
            while (true)
            {
                foreach (var i in items)
                {
                    yield return int.Parse(i);
                }
            }

        }


    }

}
