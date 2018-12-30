using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AdventOfCode2018
{
    class Day17 : Day
    {
        private bool debug = false;

        public override bool Test()
        {
            return 
                Utils.Test(Part1, testInput, "57") &&
                Utils.Test(Part2, testInput, "29");

        }

        Cell[,] grid;
        int xmax,  ymax;
        Queue<Point> heads;

        public override string Part1(string input, dynamic options)
        {
            var walls = RegexDeserializable.Deserialize<Wall>(input);

            var xmin = walls.Min(w => w.xmin) - 1;
            var ymin = walls.Min(w => w.ymin);
            xmax = walls.Max(w => w.xmax) + 1 - xmin;
            ymax = walls.Max(w => w.ymax) + 1;

            grid = new Cell[xmax, ymax];

            foreach (var wall in walls)
            {
                addWall(wall, xmin);
            }

            for (int i = 0; i < 1; i++)
            {
                int x = 500 - xmin, y = 0;
                advance(x, y, 1);
            }

            printAll();

            int count = 0;

            for (int y = 1; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    if ((get(x,y) == Cell.Water || get(x,y) == Cell.Stream) && y >= ymin)
                    {
                        count++;
                    }
                }
            }

            return count.ToString();
        }

        private void addWall(Wall wall, int xoffset)
        {
            for (int y = wall.ymin; y <= wall.ymax; y++)
            {
                for (int x = wall.xmin; x <= wall.xmax; x++)
                {
                    put(x - xoffset, y, Cell.Wall);
                }
            }
        }

        private void print(int xx, int yy)
        {
            Console.WriteLine();
            Console.WriteLine(Math.Max(0, xx - 20) + "," + Math.Max(0, yy - 8));

            for (int y = Math.Max(0, yy - 8); y < Math.Min(ymax, yy + 8); y++)
            {
                for (int x = Math.Max(0, xx - 20); x < Math.Min(xmax, xx + 20); x++)
                {
                    switch (get(x,y))
                    {
                        case Cell.Empty:
                            Console.Write(".");
                            break;
                        case Cell.Wall:
                            Console.Write("#");
                            break;
                        case Cell.Water:
                            Console.Write("~");
                            break;
                        case Cell.Stream:
                            Console.Write("|");
                            break;
                    }

                }
                Console.WriteLine();

            }

            //Console.ReadKey();

        }

        private void printAll()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    switch (get(x, y))
                    {
                        case Cell.Empty:
                            sb.Append(".");
                            break;
                        case Cell.Wall:
                            sb.Append("#");
                            break;
                        case Cell.Water:
                            sb.Append("~");
                            break;
                        case Cell.Stream:
                            sb.Append("|");
                            break;
                    }

                }
                sb.AppendLine();

            }

            // Utils.DumpToFile(sb);

        }

        private void advance(int x, int y, int dx)
        {
            //Console.WriteLine(x + "," + y + "(" + dx + ")");

            bool blocked = false;
            bool dropped = false;
            int furthest = 0;
            int startingx = x;

            var current = get(x, y);
            if (current != Cell.Empty && current != Cell.Stream)
            {
                return;
            }

            put(x, y, Cell.Stream);

            while (true)
            {
                if (y + 1 == ymax)
                {
                    return;
                }

                var below = get(x, y + 1);
                if (below == Cell.Empty )
                {
                    y++;
                    blocked = false;
                    put(x, y, Cell.Stream);
                    dropped = true;
                    continue;
                }
                else if (below == Cell.Stream)
                {
                    return;
                }

                if (dropped)
                {
                    advance(x, y, -1);
                    advance(x, y, 1);
                    return;
                }
                else
                {

                    var beside = get(x + dx, y);
                    if (beside == Cell.Empty || beside == Cell.Stream)
                    {
                        x += dx;
                        put(x, y, Cell.Stream);
                    }
                    else
                    {
                        if (blocked)
                        {
                            put(furthest, y, Cell.Water);
                            blocked = false;
                            dx *= -1;

                            if (furthest == x)
                            {
                                //print(startingx, y);
                                advance(startingx, y - 1, -1);
                                advance(startingx, y - 1, 1);
                                return;
                            }
                        }
                        else
                        {

                            blocked = true;
                            furthest = x;
                            dx *= -1;
                        }
                    }
                }
            }
        }

        private bool emptyBelow(int x, int y)
        {
            return get(x, y) == Cell.Empty;
        }

        private void put(int x, int y, Cell type)
        {
            grid[x, y] = type;
        }

        private Cell get(int x, int y)
        {
            return grid[x, y];
        }

        enum Cell
        {
            Empty,
            Wall,
            Water,
            Stream
        }

        public override string Part2(string input, dynamic options)
        {
            var walls = RegexDeserializable.Deserialize<Wall>(input);

            var xmin = walls.Min(w => w.xmin) - 1;
            var ymin = walls.Min(w => w.ymin);
            xmax = walls.Max(w => w.xmax) + 1 - xmin;
            ymax = walls.Max(w => w.ymax) + 1;

            grid = new Cell[xmax, ymax];

            foreach (var wall in walls)
            {
                addWall(wall, xmin);
            }

            for (int i = 0; i < 1; i++)
            {
                int x = 500 - xmin, y = 0;
                advance(x, y, 1);
            }

            printAll();

            int count = 0;

            for (int y = 1; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    if ((get(x, y) == Cell.Water) && y >= ymin)
                    {
                        count++;
                    }
                }
            }

            return count.ToString();
        }

        [RegexDeserializable(@"(?<direction>.)=(?<i0>\d+), .=(?<i1>\d+)\.\.(?<i2>\d+)")]
        public class Wall
        {
            public char direction;
            public int i0;
            public int i1;
            public int i2;

            public int xmin => direction == 'x' ? i0 : i1;
            public int xmax => direction == 'x' ? i0 : i2;
            public int ymin => direction == 'y' ? i0 : i1;
            public int ymax => direction == 'y' ? i0 : i2;
        }
       
        private string testInput =
@"x=495, y=2..7
y=7, x=495..501
x=501, y=3..7
x=498, y=2..4
x=506, y=1..2
x=498, y=10..13
x=504, y=10..13
y=13, x=498..504";      
    }

}

/*

    






               heads = new Queue<Point>();
            heads.Enqueue(new Point(500, 0));

            while (heads.Count > 0)
            {
                project(heads.Dequeue());            
            }




        }

        private void project(Point point)
        {
            int x = point.X, y = point.Y;
           
            while (emptyBelow(x,y))
            {
                y++;
                put(x, y, Cell.Stream);
            }

            while (detectPool(x,y))
            {
                y--;
            }


        }
*/


