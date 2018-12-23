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
    class Day20 : Day
    {
        public override bool Test()
        {
            return Utils.Test(Part1, 
                new[] { "^WNE$", "^ENWWW(NEEE|SSE(EE|N))$", "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$", "^N(N|(N|W))W((N|W))$" }, 
                new[] { "3", "10", "18", "4" });
        }

        int r0 = 0;
        static Room start;

        public override string Part1(string input, dynamic options)
        {
            Room.AllRooms.Clear();
            start = new Room() { Location = new Point(0, 0) };
            Room.AllRooms.Add(start.Location, start);

            traverse(start, input);

            print(start);

            var rooms = new Queue<Room>();
            rooms.Enqueue(start);
            start.Distance = 0;

            int d = 0;

            while (rooms.Count > 0)
            {
                var next = rooms.Dequeue();
                d = next.Distance.Value + 1;
                foreach (var neighbour in next.Neighbours)
                {
                    if (neighbour != null && neighbour.Distance == null)
                    {
                        neighbour.Distance = d;
                        rooms.Enqueue(neighbour);
                    }
                }
            }

            return (d - 1).ToString();

        }

        private void traverse(Room start, string route)
        {
            //print(start);
            //Console.ReadKey();
            Room branchpoint = null;
            var current = start;
            var depth = 0;

            for (int i = 0; i < route.Length; i++)
            {
                var c = route[i];

                switch (c)
                {
                    case '$':
                    case '^':
                        break;
                    case '(':
                        depth++;
                        if (depth == 1)
                        {
                            branchpoint = current;
                            traverse(branchpoint, route.Substring(i + 1));
                        }
                        break;
                    case '|':
                        if (depth == 0)
                        {
                            depth = 1;
                        }
                        if (depth == 1 && branchpoint != null)
                        { 
                            traverse(branchpoint, route.Substring(i + 1));
                        }
                        break;
                    case ')':
                        if (depth == 1 && branchpoint != null)
                        {
                            return;
                        }
                        depth = Math.Max(0, depth - 1);
                        break;
                    default:
                        if (depth == 0)
                        {
                            current = current.move(c);
                        }
                        break;
                }
            }
        }    
        
        static void print(Room start)
        {
            var xmin = Room.AllRooms.Min(r => r.Key.X);
            var xmax = Room.AllRooms.Max(r => r.Key.X);
            var ymin = Room.AllRooms.Min(r => r.Key.Y);
            var ymax = Room.AllRooms.Max(r => r.Key.Y);

            for (int y = ymax; y >= ymin; y--)
            {
                var nextline = new StringBuilder();

                for (int x = xmin; x <= xmax; x++)
                {
                    var p = new Point(x, y);
                    if (!Room.AllRooms.ContainsKey(p))
                    {
                        Console.Write("  ");
                        nextline.Append("  ");
                    }
                    else
                    {
                        var room = Room.AllRooms[p];
                        Console.Write(room == start ? "*" : "#");
                        if (room.Neighbours[1] != null)
                        {
                            Console.Write("-");
                        }
                        else
                        {
                            Console.Write(" ");
                        }

                        if (room.Neighbours[2] != null)
                        {
                            nextline.Append("|");
                        }
                        else
                        {
                            nextline.Append(" ");
                        }
                        nextline.Append(" ");
                    }
                }

                Console.WriteLine();
                Console.WriteLine(nextline.ToString());
            }
        }

        class Room
        {
            public static Dictionary<Point, Room> AllRooms = new Dictionary<Point, Room>();

            private static string directions = "NESW";

            public Room[] Neighbours = new Room[4];
            public Point Location;

            public int? Distance;

            public Room move(char direction)
            {
                int n = directions.IndexOf(direction);
                if (Neighbours[n] == null)
                {
                    Point adjacent = Adjacent(direction);
                    if (!AllRooms.ContainsKey(adjacent))
                    {
                        AllRooms.Add(adjacent, new Room() { Location = adjacent });
                    }
                    var room = AllRooms[adjacent];
                    Neighbours[n] = room;
                    room.Neighbours[(n + 2) % 4] = this;
                    //print(start);
                }

                return (Neighbours[n]);
            }

            private Point Adjacent(char direction)
            {
                switch (direction)
                {
                    case 'N':
                        return new Point(Location.X, Location.Y + 1);
                    case 'E':
                        return new Point(Location.X + 1, Location.Y);
                    case 'S':
                        return new Point(Location.X, Location.Y - 1);
                    case 'W':
                        return new Point(Location.X - 1, Location.Y);
                    default:
                        throw new Exception();
                }

            }
        }



        public override string Part2(string input, dynamic options)
        {
            return null;
        }

        

        private string testInput =
@"#ip 0
seti 5 0 1
seti 6 0 2
addi 0 1 0
addr 1 2 3
setr 1 0 0
seti 8 0 4
seti 9 0 5";
       
    }
}
