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

        HashSet<Point> EW = new HashSet<Point>();
        HashSet<Point> NS = new HashSet<Point>();
        Point Start = new Point(0, 0);
        string Route;

        public override string Part1(string input, dynamic options)
        {
            EW.Clear();
            NS.Clear();
            Route = input;

            var path = buildPath(1);

            traversePath(path, Start, new Stack<Path>());

            print();

            var distances = new Dictionary<Point, int>();
            distances.Add(Start, 0);
            Queue<Point> points = new Queue<Point>();
            points.Enqueue(Start);

            var distance = 0;
            while (points.Any())
            {
                var point = points.Dequeue();
                distance = distances[point] + 1;
                if (NS.Contains(point) )
                {
                    var n = new Point(point.X, point.Y + 1);
                    if (!distances.ContainsKey(n))
                    {
                        distances[n] = distance;
                        points.Enqueue(n);
                    }
                }
                var s = new Point(point.X, point.Y - 1);
                if (NS.Contains(s) && !distances.ContainsKey(s))
                {
                    distances[s] = distance;
                    points.Enqueue(s);
                }
                if (EW.Contains(point))
                {
                    var e = new Point(point.X + 1, point.Y);
                    if (!distances.ContainsKey(e))
                    {
                        distances[e] = distance;
                        points.Enqueue(e);
                    }
                }
                var w = new Point(point.X - 1, point.Y);
                if (EW.Contains(w) && !distances.ContainsKey(w))
                {
                    distances[w] = distance;
                    points.Enqueue(w);
                }
            }

            return (distance - 1).ToString();
        }

        private void traversePath(Path path, Point location, Stack<Path> subsequentPath)
        {

            var startPath = Route.Substring(path.start, path.prefixlength);
            foreach (char c in startPath)
            {
                switch (c)
                {
                    case 'N':
                        NS.Add(location);
                        location = new Point(location.X, location.Y + 1);
                        break;
                    case 'S':
                        location = new Point(location.X, location.Y - 1);
                        NS.Add(location);
                        break;
                    case 'E':
                        EW.Add(location);
                        location = new Point(location.X + 1, location.Y);
                        break;
                    case 'W':
                        location = new Point(location.X - 1, location.Y);
                        EW.Add(location);
                        break;
                }
            }

            if (path.branches.Count > 0)
            {
                foreach (var branch in path.branches)
                {
                    var subsequent = new Stack<Path>(subsequentPath);
                    subsequent.Push(path.next);
                    traversePath(branch, location, subsequent);
                }
            }
            else if (subsequentPath.Any())
            {
                var end = subsequentPath.Pop();
                traversePath(end, location, subsequentPath);
            }
        }

        private Path buildPath(int start)
        {
            Path path = new Path();
            var n = Route.IndexOfAny(new char[] { '(', '|', ')', '$' }, start);
            path.start = start;
            path.prefixlength = (n - start);

            if (Route[n] != '(')
            {
                path.end = n - 1;
                return path;
            }

            while (true)
            {
                switch (Route[n])
                {
                    case '(':
                    case '|':
                        var segment = buildPath(n + 1);
                        n = segment.end + 1;
                        path.branches.Add(segment);
                        break;
                    case ')':
                        var end = buildPath(n + 1);
                        path.next = end;
                        path.end = end.end;
                        return path;
                }
            }
        }

        class Path
        {
            public int start;
            public int prefixlength;
            public List<Path> branches = new List<Path>();
            public Path next;
            public int end;
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
        
        void print()
        {
            var xmin = EW.Min(r => r.X);
            var xmax = EW.Max(r => r.X) + 1;
            var ymin = NS.Min(r => r.Y);
            var ymax = NS.Max(r => r.Y) + 1;

            for (int y = ymax; y >= ymin; y--)
            {
                var nextline = new StringBuilder();

                for (int x = xmin; x <= xmax; x++)
                {
                    var p = new Point(x, y);
                    var below = new Point(p.X, p.Y - 1);
                    var room = EW.Contains(p)
                        || EW.Contains(new Point(p.X - 1, p.Y))
                        || NS.Contains(p)
                        || NS.Contains(below);
                    if (!room)
                    {
                        Console.Write("  ");
                        nextline.Append("  ");
                    }
                    else
                    {                        
                        Console.Write(p == Start ? "*" : "#");
                        if (EW.Contains(p))
                        {
                            Console.Write("-");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        if (NS.Contains(below))
                        {
                            nextline.Append("| ");
                        }
                        else
                        {
                            nextline.Append("  ");
                        }                        
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
