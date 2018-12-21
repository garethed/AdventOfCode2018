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
    class Day15 : Day
    {
        private bool debug = false;

        public override bool Test()
        {
            return Utils.Test(Part1, testInputs, testOutputs) &&
            Utils.Test(Part2, testInputs, new string[] { "29064", "31284", "3478", "6474", "1140" });

        }

        private Dictionary<Point, object> board;
        private Dictionary<Point,int> distancePrototype;
        private List<Unit> units;
        private int elfPower = 3;
        private bool deadElves = false;
        private int rounds;
        private int w, h;

        public override string Part1(string input, dynamic options)
        {
            var boarddata = Utils.splitLines(input).ToList();
            w = boarddata[0].Length;
            h = boarddata.Count;

            board = new Dictionary<Point, object>();
            distancePrototype = new Dictionary<Point, int>();
            units = new List<Unit>();
            deadElves = false;


            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    object cell = null;

                    switch (boarddata[y][x])
                    {
                        case '#':
                            cell = new Wall();
                            break;
                        case 'E':
                        case 'G':
                            var unit = new Unit() { type = boarddata[y][x], location = new Point(x, y) };
                            units.Add(unit);
                            cell = unit;
                            break;

                    }

                    var point = new Point(x, y);

                    board[point] = cell;
                    distancePrototype[point] = int.MaxValue;
                }
            }

            for (rounds = 0; ; rounds++)
            {
                units = units.OrderBy(u => u.location.Y * 1000 + u.location.X).ToList();

                foreach (var unit in units)
                {
                    if (unit.hitPoints <= 0) continue;

                    var distances = buildDistanceMap(board, unit.location);
                    var potentialTargets = units.Where(u => u.type != unit.type && u.hitPoints > 0);

                    if (!potentialTargets.Any())
                    {
                        if (debug) print(board, w, h);
                        Console.WriteLine(rounds + " x " + units.Where(u => u.hitPoints > 0).Sum(u => u.hitPoints));
                        return (rounds * units.Where(u => u.hitPoints > 0).Sum(u => u.hitPoints)).ToString();
                    }

                    var inrange = potentialTargets.SelectMany(u => u.inRange).Where(p => distances[p] != int.MaxValue).OrderBy(p => distances[p]).ThenBy(p => 1000 * p.Y + p.X);

                    if (inrange.Any() && !inrange.Contains(unit.location))
                    {
                        var movetarget = inrange.First();
                        var movemap = buildDistanceMap(board, movetarget);
                        var move = unit.inRange.OrderBy(p => movemap[p]).ThenBy(p => p.Y * 1000 + p.X).First();

                        var old = unit.location;
                        board[old] = null;
                        board[move] = unit;
                        unit.location = move;
                        if (debug) print(board, w, h);
                    }

                    var attackTarget = unit.inRange.Select(p => board[p] as Unit).Where(u => u != null && u.type != unit.type).OrderBy(u => u.hitPoints).FirstOrDefault();
                    if (attackTarget != null)
                    {
                        attackTarget.hitPoints -= (unit.type == 'G' ? 3 : elfPower);
                        if (attackTarget.hitPoints <= 0)
                        {
                            board[attackTarget.location] = null;
                            if (debug) print(board, w, h);

                            if (attackTarget.type == 'E')
                            {
                                deadElves = true;
                            }
                        }
                    }
                }

                //print(board, w, h);
            }

            
        }

        private void print(Dictionary<Point, object> board, int w, int h)
        {
            Console.WriteLine("\n");

            for (int y = 0; y < h; y++)
            {
                var health = "   ";

                for (int x = 0; x < w; x++)
                {
                    var cell = board[new Point(x, y)];

                    if (cell is Wall)
                    {
                        Console.Write("#");
                    }
                    else if (cell is Unit)
                    {
                        var unit = cell as Unit;
                        Console.Write(unit.type);
                        health += unit.type + ":" + unit.hitPoints + " ";
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine(health);
            }
        }

        private Dictionary<Point,int> buildDistanceMap(Dictionary<Point,object> board, Point location)
        {
            var map = new Dictionary<Point, int>(distancePrototype);

            var points = new Queue<Point>();
            points.Enqueue(location);
            map[location] = 0;

            while (points.Count > 0)
            {
                var point = points.Dequeue();
                int value = map[point] + 1;
                foreach (var neighbour in neighbours(point))
                {
                    if (board[neighbour] == null && map[neighbour] > value)
                    {
                        map[neighbour] = value;
                        points.Enqueue(neighbour);

                    }
                }
            }

            return map;
        }

        private static IEnumerable<Point> neighbours(Point location)
        {
            yield return new Point(location.X, location.Y - 1);
            yield return new Point(location.X - 1, location.Y);
            yield return new Point(location.X + 1, location.Y);
            yield return new Point(location.X, location.Y + 1);

        }

        private int distance(Point location, Point p, object[,] board)
        {
            throw new NotImplementedException();
        }

        public override string Part2(string input, dynamic options)
        {
            int lbound = 3;
            int ubound = 6;
            var score = 0;

            while (!test(input, ubound))
            {
                ubound *= 2;
            }

            score = (rounds * units.Where(u => u.hitPoints > 0).Sum(u => u.hitPoints));

            while (ubound - lbound > 1)
            {
                var mid = (ubound + lbound) / 2;
                if (test(input, mid))
                {
                    ubound = mid;
                    score = (rounds * units.Where(u => u.hitPoints > 0).Sum(u => u.hitPoints));
                }
                else
                {
                    lbound = mid;
                }
            }

            return score.ToString();


        }

        public bool test(string input, int elfpower)
        {           

            this.elfPower = elfpower;
            Part1(input, null);

            Console.WriteLine("p:" + elfpower + " -> " + (deadElves ? "dead" : "alive"));

            return !deadElves;
        }

        

        private class Wall
        { }

        private class Unit
        {
            public char type;
            public Point location;
            public int hitPoints = 200;

            public IEnumerable<Point> inRange => neighbours(location);            
        }

        private string[] testOutputs = new[] { "36334", "39514", "27755", "28944", "18740" };

        private string[] testInputs = new[] {
        @"#######
#G..#E#
#E#E.E#
#G.##.#
#...#E#
#...E.#
#######",

@"####### 
#E..EG#
#.#G.E#
#E.##E#
#G..#.#
#..E#.#
#######",


@"#######
#E.G#.#
#.#G..#
#G.#.G#
#G..#.#
#...E.#
#######",

@"#######
#.E...#
#.#..G#
#.###.#
#E#G#G#
#...#G#
#######",

@"#########
#G......#
#.E.#...#
#..##..G#
#...##..#
#...#...#
#.G...G.#
#.....G.#
#########" };                      

    }

}
