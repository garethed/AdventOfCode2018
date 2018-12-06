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
    class Day6 : Day
    {
        int[,] grid;
        int xmax;
        int ymax;
        int[] counts;

        public override void Test()
        {
            Utils.Test(Part1, "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9", "17" );
            //Utils.Test(Part2, "dabAcCaCBAcCcaDA", "4");
        }

     
        public override string Part1(dynamic input)
        {
            var points = Utils.splitLines((string)input).Select(parseLine).ToList();
            xmax = points.Max(p => p.X);
            ymax = points.Max(p => p.Y);

            grid = new int[xmax + 2,ymax + 2];
            counts = new int[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                put(points[i], i + 1);
            }

            while (Propagate())
            {
            }

            Print();

            return (1 + counts.Max()).ToString();
        }

        private bool Propagate()
        {
            int[,] newgrid = (int[,])grid.Clone();
            bool updated = false;

            for (int x = 1; x <= xmax; x++)
            {
                for (int y = 1; y <= ymax; y++)
                {
                    if (grid[x, y] == 0)
                    {
                        var newValue = MostCommonNeighbour(x, y);
                        newgrid[x, y] = newValue;
                        if (newValue != 0)
                        {
                            updated = true;
                            if (newValue > 0)
                            {
                                counts[newValue - 1]++;
                                if (x == 1 || y == 1 || x == xmax || y == ymax)
                                {
                                    counts[newValue - 1] = int.MinValue;
                                }
                            }
                        }
                    }
                }
            }
            
            grid = newgrid;           
            return updated;
        }

        private void Print()
        {
            if (xmax < 20) return;

            StringBuilder sb = new StringBuilder();
            
            for (int y = 1; y <= ymax; y++)
            {
                sb.Append("\n");
                for (int x = 1; x <= xmax; x++)
                {
                    if (grid[x, y] < 0)
                    {
                        sb.Append(".");
                    }
                    else if (grid[x, y] == 0)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.Append((char) (grid[x, y]  + 32));
                    }
                    //Console.Write(" ");
                }
            }

            string temp = System.IO.Path.GetTempFileName().Replace(".tmp", ".txt");
            System.IO.File.WriteAllText(temp, sb.ToString());
            Process.Start(temp);


        }

        private int MostCommonNeighbour(int x, int y)
        {
            var neighbours = new List<int>();
            neighbours.Add(grid[x - 1, y]);
            neighbours.Add(grid[x + 1, y]);
            neighbours.Add(grid[x, y - 1]);
            neighbours.Add(grid[x, y + 1]);

            var grouped = neighbours.Where(n => n != 0).GroupBy(n => n).OrderByDescending(g => g.Count()).ToList();

            if (grouped.Count() == 0)
            {
                return 0;
            }
            if (grouped.Count() == 1)
            {
                return grouped[0].Key;
            }
            if (grouped[0].Count() != grouped[1].Count())
            {
                return grouped[0].Key;
            }

            return -1;
        }

        private void put(Point point, int v)
        {
            grid[point.X, point.Y] = v;
        }

        public override string Part2(dynamic input)
        {
            return null;
        }        

        private Point parseLine(string line)
        {
            var parts = line.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            return new Point(parts[0], parts[1]);
        }
    }

}
