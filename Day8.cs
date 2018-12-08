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
    class Day8 : Day
    {

        public override void Test()
        {
            Utils.Test(Part1, "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2", "138");
            Utils.Test(Part2, "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2", "66");
        }

        public override dynamic Options => new { basetime = 60, elves = 5 };

        public override string Part1(string input, dynamic options)
        {
            var data = input.Split(' ').Select(i => int.Parse(i)).ToList();

            return consumeNode(data, 0).Item1.ToString();
        }

        private (int,int,int) consumeNode(List<int> data, int offset)
        {
            var originalOffset = offset;
            var nodes = data[offset];
            var metadata = data[offset + 1];

            var total = 0;
            var alternateTotal = 0;
            var childTotals = new int[nodes];

            offset += 2;

            for (int n = 0; n < nodes; n++)
            {
                (var metadatadelta, var alternateDelta, var newOffset) = consumeNode(data, offset);
                total += metadatadelta;
                offset = newOffset;
                childTotals[n] = alternateDelta;
            }

            for(int m = 0;m < metadata;m++)
            {
                total += data[offset];
                if (data[offset] > 0 && data[offset] < nodes + 1)
                {
                    alternateTotal += childTotals[data[offset] - 1];
                }
                offset++;
            }

            return (total, nodes > 0 ? alternateTotal : total, offset);
        }

        public override string Part2(string input, dynamic options)
        {
            var data = input.Split(' ').Select(i => int.Parse(i)).ToList();

            return consumeNode(data, 0).Item2.ToString();
        }

    }

}
