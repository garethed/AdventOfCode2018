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
    class Day18 : Day
    {
        private bool debug = false;

        public override bool Test()
        {
            return
                Utils.Test(Part1, testInput, "1147"); 
        }

        Cell[,] grid;
        int xmax,  ymax;


        public override string Part1(string input, dynamic options)
        {
            var data = Utils.splitLines(input).ToList();
            xmax = data.First().Length;
            ymax = data.Count();

            int rounds = (options as int?) ?? 10;

            grid = new Cell[xmax + 2, ymax + 2];

            for (int y = 0; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    switch (data[y][x])
                    {
                        case '#':
                            grid[x + 1, y + 1] = Cell.Lumberyard;
                            break;
                        case '|':
                            grid[x + 1, y + 1] = Cell.Forest;
                            break;
                    }
                }

            }

            //print(10, 8);

            for (int i = 0; i < rounds; i++)
            {
                grid = advance(grid);
                //print(10, 8);
                Console.WriteLine((grid.flatten().Count(c => c == Cell.Forest) * grid.flatten().Count(c => c == Cell.Lumberyard)).ToString());
            }

            return (grid.flatten().Count(c => c == Cell.Forest) * grid.flatten().Count(c => c == Cell.Lumberyard)).ToString();
        }


        private void print(int xx, int yy)
        {
            Console.WriteLine();
            Console.WriteLine(Math.Max(0, xx - 20) + "," + Math.Max(0, yy - 8));

            for (int y = Math.Max(0, yy - 8); y < Math.Min(ymax, yy + 8); y++)
            {
                for (int x = Math.Max(0, xx - 20); x < Math.Min(xmax, xx + 20); x++)
                {
                    switch (grid[x,y])
                    {
                        case Cell.Empty:
                            Console.Write(".");
                            break;
                        case Cell.Lumberyard:
                            Console.Write("#");
                            break;
                        case Cell.Forest:
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
                    switch (grid[x + 1, y + 1])
                    {
                        case Cell.Empty:
                            sb.Append(".");
                            break;
                        case Cell.Lumberyard:
                            sb.Append("#");
                            break;
                        case Cell.Forest:
                            sb.Append("|");
                            break;
                    }

                }
                sb.AppendLine();

            }

            Utils.DumpToFile(sb);

        }

        private Cell[,] advance(Cell[,] grid)
        {
            var output = (Cell[,])grid.Clone();


            for (int y = 1; y < ymax + 1; y++)
            {
                for (int x = 1; x < xmax + 1; x++)
                {
                    var forests = neighbours(grid, x, y).Count(c => c == Cell.Forest);
                    var lumberyards = neighbours(grid, x, y).Count(c => c == Cell.Lumberyard);

                    switch (grid[x,y])
                    {
                        case Cell.Empty:
                            if (forests >= 3)
                            {
                                output[x, y] = Cell.Forest;
                            }
                            break;
                        case Cell.Forest:
                            if (lumberyards >= 3)
                            {
                                output[x, y] = Cell.Lumberyard;
                            }
                            break;
                        case Cell.Lumberyard:
                            if (lumberyards == 0 || forests == 0)
                            {
                                output[x, y] = Cell.Empty;
                            }
                            break;
                    }
                }
            }

            return output;
        }

        private IEnumerable<Cell> neighbours(Cell[,] grid, int x, int y)
        {
            yield return grid[x - 1, y - 1];
            yield return grid[x , y - 1];
            yield return grid[x + 1, y - 1];
            yield return grid[x - 1, y];
            yield return grid[x + 1, y];
            yield return grid[x - 1, y + 1];
            yield return grid[x, y + 1];
            yield return grid[x + 1, y + 1];

        }

        enum Cell
        {
            Empty,
            Forest,
            Lumberyard
        }

        public override string Part2(string input, dynamic options)
        {
            Part1(input, (int?)1000);

            var example = (Cell[,])grid.Clone();

            int steps = 0;
            while (true)
            {
                grid = advance(grid);
                steps++;

                if (example.flatten().SequenceEqual(grid.flatten()))
                {
                    break;
                }
            }

            // repeat every [steps] steps
            int remainingsteps = (1000000000 - 1000) % steps;

            for (int s = 0; s < remainingsteps; s++)
            {
                grid = advance(grid);
            }


            return (grid.flatten().Count(c => c == Cell.Forest) * grid.flatten().Count(c => c == Cell.Lumberyard)).ToString();



        }


        private string testInput =
@".#.#...|#.
.....#|##|
.|..|...#.
..|#.....#
#.#|||#|#|
...#.||...
.|....|...
||...#|.#|
|.||||..|.
...#.|..|.";      
    }

}
