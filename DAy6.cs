﻿using System;
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

        public override bool Test()
        {
            return Utils.Test(Part1, "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9", "17" ) &&
            Utils.Test(Part2,  "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9", "16", new { max = 32 });
        }

        public override dynamic Options => new { max = 10000 };

        public override string Part1(string input, dynamic options)
        {
            var points = Utils.splitLines((string)input).Select(parseLine).ToList();
            xmax = points.Max(p => p.X);
            ymax = points.Max(p => p.Y);

            grid = new int[xmax + 2,ymax + 2];
            counts = new int[points.Count];

            for (int x = 1; x <= xmax; x++)
            {
                for (int y = 1; y <= ymax; y++)
                {
                    var newValue = NearestRoot(x, y, points);
                    grid[x, y] = newValue; 
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

            //Print();

            return (counts.Max()).ToString();
        }

        private int NearestRoot(int x, int y, List<Point> points)
        {
            int minDistance = int.MaxValue;
            int closest = 0;
            bool multipleClosest = false;

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                var distance = Math.Abs(x - p.X) + Math.Abs(y - p.Y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = i;
                    multipleClosest = false;
                }
                else if (distance == minDistance)
                {
                    multipleClosest = true;
                }
            }

            return multipleClosest ? -1 : closest + 1;
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

        public override string Part2(string input, dynamic options)
        {
            var points = Utils.splitLines((string)input).Select(parseLine).ToList();
            xmax = points.Max(p => p.X);
            ymax = points.Max(p => p.Y);
            var threshold = options.max;

            grid = new int[xmax + 2, ymax + 2];

            for (int x = 1; x <= xmax; x++)
            {
                for (int y = 1; y <= ymax; y++)
                {
                    var distance = points.Sum(p => Math.Abs(p.X - x) + Math.Abs(p.Y - y));
                    grid[x, y] = distance;                
                }
            }

            return grid.flatten().Count(x => x > 0 && x < threshold).ToString();
        }

        private Point parseLine(string line)
        {
            var parts = line.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            return new Point(parts[0], parts[1]);
        }
    }

}
