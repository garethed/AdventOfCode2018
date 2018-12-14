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
    class Day9 : Day
    {

        public override void Test()
        {
            Utils.Test(Part1, "", "32", new { max = 27, elves = 9 });
            Utils.Test(Part1, "", "8317", new { max = 1618, elves = 10 });
            Utils.Test(Part1, "", "146373", new { max = 7999, elves = 13 });
            Utils.Test(Part1, "", "2764", new { max = 1104, elves = 17 });
            Utils.Test(Part1, "", "54718", new { max = 6111, elves = 21 });
            Utils.Test(Part1, "", "37305", new { max = 5807, elves = 30 });

            //Utils.Test(Part2, "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2", "66");
        }

        public override dynamic Options => new { max = 71626, elves = 438 };

        public override string Part1(string input, dynamic options)
        {
            LinkedList<long> circle = new LinkedList<long>();
            long[] scores = new long[options.elves];

            var current = circle.AddFirst(0);
            current = circle.AddAfter(current, 1);

            for (int m = 2; m <= options.max; m++)
            {
                if (m % 23 != 0)
                {
                    current = Move(current, 1);
                    current = circle.AddAfter(current, m);
                }
                else
                {
                    current = Move(current, -6);
                    var toRemove = current.Previous ?? circle.Last;
                    circle.Remove(toRemove);

                    scores[m % options.elves] += m + toRemove.Value;
                }

                //Print(current);
            }

            return scores.Max().ToString();
        }

        private void Print(LinkedListNode<int> current)
        {
            Console.Write(current.Value + ": ");

            foreach (var node in current.List)
            {
                Console.Write(node.ToString("00") + " ");
            }

            Console.WriteLine();
        }

        private LinkedListNode<T> Move<T>(LinkedListNode<T> start, int count)
        {
            var node = start;
            for (int i = 0; i < Math.Abs(count); i++)
            {
                if (count > 0)
                {
                    node = node.Next;
                    if (node == null)
                    {
                        node = start.List.First;
                    }
                }
                else
                {
                    node = node.Previous;
                    if (node == null)
                    {
                        node = start.List.Last;
                    }

                }
            }

            return node;

        }


        public override string Part2(string input, dynamic options)
        {
            return Part1(input, new { elves = options.elves, max = options.max * 100 });
        }

    }

}
