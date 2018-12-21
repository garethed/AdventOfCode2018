using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    class Day2 : Day
    {

        public override bool Test()
        {
            return Utils.Test(Part1, "abcdef\nbababc\nabbcde\nabcccd\naabcdd\nabcdee\nababab" , "12" ) &&
            Utils.Test(Part2, "abcde\nfghij\nklmno\npqrst\nfguij\naxcye\nwvxyz", "fgij");
        }

        public override string Part1(string input, dynamic options)
        {
            int twos = 0, threes = 0;

            foreach (var s in Utils.splitLines(input))
            {
                LineChecksum(s, out int two, out int three);
                twos += two;
                threes += three;
            }

            return (twos * threes).ToString();              
        }

        public override string Part2(string input, dynamic options)
        {
            int i = 0;

            while (true)
            {
                var match = Utils.splitLines((string)input).Select(l => l.Remove(i, 1)).GroupBy(l => l).Where(g => g.Count() == 2).FirstOrDefault();

                if (match != null)
                {
                    return match.Key;
                }

                i++;
            }

        
        }

        private void LineChecksum(string s, out int twos, out int threes)
        {
            twos = 0;
            threes = 0;

            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            foreach (var letter in alphabet)
            {
                int found = 0;
                foreach (var c in s)
                {
                    if (c == letter)
                    {
                        found++;
                    }
                    if (found > 3)
                    {
                        break;
                    }
                }
                
                if (found == 2)
                {
                    twos = 1;
                }
                else if (found == 3)
                {
                    threes = 1;
                }

                if (twos == 1 && threes == 1)
                {
                    break;
                }
            }
        }

        
    }

}
