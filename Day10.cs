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
    class Day10 : Day
    {

        public override bool Test()
        {
            return Utils.Test(Part1, testInput, "3");
            //Utils.Test(Part2, "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2", "66");
        }


        public override string Part1(string input, dynamic options)
        {
            var data = Utils.splitLines(input).Select(i => new Particle(i)).ToList();

            var minbounds = int.MaxValue;
            string output = null;

            int initial = (data[0].p.X - data[1].p.X) / (data[1].v.X - data[0].v.X);

            int ticks = 0;

            if (initial > 500)
            {
                foreach (var p in data)
                {
                    p.Tick(initial - 500);                    
                }
                ticks += (initial - 500);
            }


            

            while (true)
            {
                var bounds = (data.Max(d => d.p.X) - data.Min(d => d.p.X)) *
                    (data.Max(d => d.p.Y) - data.Min(d => d.p.Y));

                if (bounds < minbounds)
                {
                    if (bounds < 10000)
                    {
                        output = print(data);
                    }
                    minbounds = bounds;
                }
                else 
                {
                    Console.WriteLine(output);
                    return (ticks - 1).ToString();
                }

                foreach (var p in data)
                {
                    p.Tick(1);
                }

                ticks++;
            }
        }

        private string print(List<Particle> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();

            for (int y = data.Min(d => d.p.Y); y <= data.Max(d => d.p.Y); y++)
            {
                for (int x = data.Min(d => d.p.X); x <= data.Max(d => d.p.X); x++)
                {
                    if (data.Any(p => p.p.X == x && p.p.Y == y))
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public override string Part2(string input, dynamic options)
        {
            return null;
        }



        private string testInput =
@"position=< 9,  1> velocity=< 0,  2>
position=< 7,  0> velocity=<-1,  0>
position=< 3, -2> velocity=<-1,  1>
position=< 6, 10> velocity=<-2, -1>
position=< 2, -4> velocity=< 2,  2>
position=<-6, 10> velocity=< 2, -2>
position=< 1,  8> velocity=< 1, -1>
position=< 1,  7> velocity=< 1,  0>
position=<-3, 11> velocity=< 1, -2>
position=< 7,  6> velocity=<-1, -1>
position=<-2,  3> velocity=< 1,  0>
position=<-4,  3> velocity=< 2,  0>
position=<10, -3> velocity=<-1,  1>
position=< 5, 11> velocity=< 1, -2>
position=< 4,  7> velocity=< 0, -1>
position=< 8, -2> velocity=< 0,  1>
position=<15,  0> velocity=<-2,  0>
position=< 1,  6> velocity=< 1,  0>
position=< 8,  9> velocity=< 0, -1>
position=< 3,  3> velocity=<-1,  1>
position=< 0,  5> velocity=< 0, -1>
position=<-2,  2> velocity=< 2,  0>
position=< 5, -2> velocity=< 1,  2>
position=< 1,  4> velocity=< 2,  1>
position=<-2,  7> velocity=< 2, -2>
position=< 3,  6> velocity=<-1, -1>
position=< 5,  0> velocity=< 1,  0>
position=<-6,  0> velocity=< 2,  0>
position=< 5,  9> velocity=< 1, -2>
position=<14,  7> velocity=<-2,  0>
position=<-3,  6> velocity=< 2, -1>";

        private class Particle
        {
            public Point p;
            public Point v;

            public Particle(int x, int y, int dx, int dy)
            {
                p = new Point(x, y);
                v = new Point(dx, dy);
            }

            public Particle(string line)
            {
                line = line.Replace("> v", ",");
                line = Regex.Replace(line, "[^0-9-,]", "");
                var parts = line.Split(',').Select(s => int.Parse(s)).ToList();
                p = new Point(parts[0], parts[1]);
                v = new Point(parts[2], parts[3]);
            }

            public void Tick(int delta)
            {
                p.Offset(v.X * delta, v.Y * delta);
            }
        }
    }

}
