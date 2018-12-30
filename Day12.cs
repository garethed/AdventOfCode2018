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
    class Day12 : Day
    {
        int offset = 10;
        int generations = 200;
        int size = 400;

        public override bool Test()
        {
            return Utils.Test(Part1, testInput, "325");
        }


        public override string Part1(string input, dynamic options)
        {
            var statestring = Utils.splitLines(input).First().Split(' ')[2];
            bool[] state = new bool[size];
            
            
            for (int i = 0; i < statestring.Length; i++)
            {
                if (statestring[i] == '#')
                {
                    state[offset + i] = true;
                }
            }

            var rulesstrings = Utils.splitLines(input).Skip(2).ToList();
            var rules = new bool[rulesstrings.Count,6];

            for (int r = 0; r < rulesstrings.Count; r++)
            {
                for (int i = 0; i < 5; i++)
                {
                    rules[r, i] = (rulesstrings[r][i] == '#');
                    rules[r, 5] = (rulesstrings[r][9] == '#');
                }
            }

            for (int g = 0; g < 20; g++)
            {
                bool[] newState = new bool[size];

                for (int x = 0; x < state.Length - 4; x++)
                {
                    for (int r = 0; r < rulesstrings.Count; r++)
                    {
                        bool matches = true;
                        for (int i = 0; i < 5; i++)
                        {
                            matches &= (rules[r,i] == state[x + i]);
                        }

                        if (matches)
                        {
                            newState[x + 2] = rules[r, 5];
                            break;
                        }
                        
                    }                    
                }

                state = newState;
            }

            Console.WriteLine();

            return Total(state).ToString();

        }

        private void print(bool[] state)
        {
            Console.WriteLine();
            foreach (bool b in state)
            {
                Console.Write(b ? "#" : ".");
            }
        }

        private int Total(bool[] state)
        {
            int total = 0;

            for (int x = 0; x < state.Length; x++)
            {
                if (state[x])
                {
                    total += (x - offset);
                }
            }

            return total;
        }

        public override string Part2(string input, dynamic options)
        {
            var statestring = Utils.splitLines(input).First().Split(' ')[2];
            bool[] state = new bool[size];


            for (int i = 0; i < statestring.Length; i++)
            {
                if (statestring[i] == '#')
                {
                    state[offset + i] = true;
                }
            }

            var rulesstrings = Utils.splitLines(input).Skip(2).ToList();
            var rules = new bool[rulesstrings.Count, 6];

            for (int r = 0; r < rulesstrings.Count; r++)
            {
                for (int i = 0; i < 5; i++)
                {
                    rules[r, i] = (rulesstrings[r][i] == '#');
                    rules[r, 5] = (rulesstrings[r][9] == '#');
                }
            }

            int prevTotal = Total(state);
            int prevDelta = 0;

            for (int g = 0; g < 200; g++)
            {
                bool[] newState = new bool[size];

                for (int x = 0; x < state.Length - 4; x++)
                {
                    for (int r = 0; r < rulesstrings.Count; r++)
                    {
                        bool matches = true;
                        for (int i = 0; i < 5; i++)
                        {
                            matches &= (rules[r, i] == state[x + i]);
                        }

                        if (matches)
                        {
                            newState[x + 2] = rules[r, 5];
                            break;
                        }

                    }
                }

                state = newState;

                //print(state);
                var total = Total(state);
                var delta = total - prevTotal;
                Console.Write(" " + total + " " + delta);

                if (prevDelta == delta)
                {
                    Console.WriteLine("converged after " + g);
                    return ((long)total + (long)delta * (50000000000l - g - 1)).ToString();
                }

                prevTotal = total;
                prevDelta = delta;
            }

            Console.WriteLine();

            return Total(state).ToString();

        }

        private string testInput =>
@"initial state: #..#.#..##......###...###

...## => #
..#.. => #
.#... => #
.#.#. => #
.#.## => #
.##.. => #
.#### => #
#.#.# => #
#.### => #
##.#. => #
##.## => #
###.. => #
###.# => #
####. => #";
    }

}
