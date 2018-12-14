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
    class Day13 : Day
    {
        public override void Test()
        {
            Utils.Test(Part1, testInput, "7,3");
            Utils.Test(Part2, testInput2, "6,4");
        }



        public override string Part1(string input, dynamic options)
        {
            var track = Utils.splitLinesWithoutTrim(input).ToArray();
            var cars = initCars(track);

            try
            {
                while (true)
                {
                    var ordered = cars.OrderBy(c => c.SortOrder).ToList();

                    foreach (var car in ordered)
                    {
                        car.Move(ordered);
                    }
                }
            }
            catch (Crash c)
            {
                return c.X + "," + c.Y;
            }
        }

        private List<Car> initCars(string[] track)
        {
            var cars = new List<Car>();

            for (int x = 0; x < track[0].Length; x++)
            {
                for (int y = 0; y < track.Length; y++)
                {
                    int dx = 0, dy = 0;
                    switch (track[y][x])
                    {
                        case '>':
                            dx = 1;
                            break;
                        case '<':
                            dx = -1;
                            break;
                        case '^':
                            dy = -1;
                            break;
                        case 'v':
                            dy = 1;
                            break;
                    }

                    if (dx != 0 || dy != 0)
                    {
                        cars.Add(new Car { X = x, Y = y, dX = dx, dY = dy, track = track });
                    }
                }
            }

            return cars;
        }

        public override string Part2(string input, dynamic options)
        {
            var track = Utils.splitLinesWithoutTrim(input).ToArray();
            var cars = initCars(track);

                while (true)
                {
                    var ordered = cars.OrderBy(c => c.SortOrder).ToList();

                    foreach (var car in ordered)
                    {
                        try
                        {
                            car.Move(ordered);
                        }
                        catch (Crash c) { }
                    }

                    var alive = cars.Where(c => c.alive);

                    if (alive.Count() == 1)
                    {
                        return alive.First().X + "," + alive.First().Y;
                    }                                           
                }
        }

        private string testInput =>
@"/->-\        
|   |  /----\
| /-+--+-\  |
| | |  | v  |
\-+-/  \-+--/
  \------/   ";

        private string testInput2 =>
@"/>-<\  
|   |  
| /<+-\
| | | v
\>+</ |
  |   ^
  \<->/";

        private class Car
        {
            public string[] track;

            public int X;
            public int Y;

            public int dX;
            public int dY;

            int turnCount = 0;

            public bool alive = true;

            public int SortOrder => Y * 1000 + X;

            public void Move(List<Car> otherCars)
            {
                if (!alive) return;

                X += dX;
                Y += dY;

                /*if (otherCars.First() == this)
                {
                    Console.WriteLine();
                }

                Console.Write(X.ToString("00") + "," + Y.ToString("00") + " ");*/

                var collision = otherCars.FirstOrDefault(c => c.X == X && c.Y == Y && c != this && c.alive);

                if (collision != null)
                {
                    this.alive = false;
                    collision.alive = false;
                    Console.WriteLine("crash " + X + "," + Y);
                    throw new Crash() { X = X, Y = Y };
                }

                var dX2 = dX;
                var dY2 = dY;

                switch (track[Y][X])
                {
                    case '-':
                    case '|':
                    case '<':
                    case '>':
                    case '^':
                    case 'v':
                        break;
                    case '/':
                        dX = -dY2;
                        dY = -dX2;
                        break;
                    case '\\':
                        dX = dY2;
                        dY = dX2;
                        break;
                    case '+':
                        switch (turnCount % 3)
                        {
                            case 0:
                                dX = dY2;
                                dY = -dX2;
                                break;
                            case 1:
                                break;
                            case 2:
                                dX = -dY2;
                                dY = dX2;
                                break;
                        }
                        turnCount++;
                        break;
                    default:

                        throw new Exception();
                }
            }
        }

        private class Crash : Exception
        {
            public int X;
            public int Y;
        }

    }

}
