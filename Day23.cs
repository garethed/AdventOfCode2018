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
    class Day23 : Day
    {
        public override bool Test()
        {
            return Utils.Test(Part1, testInput, "7");
        }

        public override string Part1(string input, dynamic options)
        {
            var bots = RegexDeserializable.Deserialize<bot>(input);

            var mostpowerful = bots.OrderByDescending(b => b.r).First();
            return bots.Where(b => mostpowerful.inRangeFromMe(b)).Count().ToString();
        }

        public override string Part2(string input, dynamic options)
        {
            var bots = RegexDeserializable.Deserialize<bot>(input);

            var xmin = bots.Min(b => b.x);
            var xmax = bots.Max(b => b.x);
            var ymin = bots.Min(b => b.y);
            var ymax = bots.Max(b => b.y);
            var zmin = bots.Min(b => b.z);
            var zmax = bots.Max(b => b.z);

            var mostdense = map(bots, new cell ( xmin, xmax, ymin, ymax, zmin, zmax, 10000000));
            cell best = null;

            while (true)
            {
                best = null;
                List<cell> next = null;
                cell bestparent = null;

                foreach (var cell in mostdense)
                {
                    if (best != null && cell.density < best.density)
                    {

                        break;
                    }

                    var subcells = map(bots, cell);
                    var subcell = subcells.First();
                    if (best == null || subcell.density > best.density || (subcell.density == best.density && subcell.distance < best.distance))
                    {
                        best = subcell;
                        next = subcells;
                        bestparent = cell;
                    }
                }

                if (best.scale == 0)
                {
                    Console.WriteLine("final location was " + best.xmin + "," + best.ymin + "," + best.zmin + " at scale " + best.scale + " with density " + best.density);
                    return best.distance.ToString();
                }

                Console.WriteLine("best cell was " + bestparent.xmin + "," + bestparent.ymin + "," + bestparent.zmin + " at scale " + bestparent.scale + " with density " + bestparent.density);
                mostdense = next;
            }
        }

        private List<cell> map(List<bot> bots, cell cell)
        {
            return cell.subcells(bots).ToList().OrderByDescending(c => c.density).ToList();
        }

        public class cell
        {
            public int xmin;
            public int xmax;
            public int ymin;
            public int ymax;
            public int zmin;
            public int zmax;
            public int scale;
            public int density;
            public int distance;

            public int xmid;
            public int ymid;
            public int zmid;

            public cell(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, int scale)
            {
                this.xmin = xmin;
                this.ymin = ymin;
                this.zmin = zmin;
                this.xmax = xmax;
                this.ymax = ymax;
                this.zmax = zmax;
                this.scale = scale;
                this.distance = Math.Abs(xmin) + Math.Abs(ymin) + Math.Abs(zmin);
                xmid = (xmin + xmax) / 2;
                ymid = (ymin + ymax) / 2;
                zmid = (zmin + zmax) / 2;
            }

            public IEnumerable<cell> subcells(List<bot> bots)
            {
                for (int x = xmin; x < xmax; x += scale)
                {
                    for (int y = ymin; y < ymax; y += scale)
                    {
                        for (int z = zmin; z < zmax; z += scale)
                        {
                            var cell = new cell(x, x + scale - 1, y, y + scale - 1, z, z + scale - 1, scale / 10);
                            cell.calculateDensity(bots);
                            yield return cell;
                        }
                    }

                }
            }

            public void calculateDensity(List<bot> bots)
            {
                foreach (var bot in bots)
                {
                    var distance = minDistance(bot.x, xmin, xmax) + minDistance(bot.y, ymin, ymax) + minDistance(bot.z, zmin, zmax);
                    if (distance <= bot.r || this.contains(bot))
                    {
                        density++;
                    }
                }
            }

            private bool contains(bot bot)
            {
                return bot.x >= xmin && bot.x <= xmax && bot.y >= ymin && bot.y <= ymax && bot.z >= zmin && bot.z <= zmax;
            }

            private int minDistance(int from, int low, int hi)
            {
                return Math.Min(Math.Abs(from - low), Math.Abs(from - hi));
            }
        }



        [RegexDeserializable(@"pos=<(?<x>-?\d+)\,(?<y>-?\d+),(?<z>-?\d+)>, r=(?<r>\d+)")]
        public class bot
        {
            public int x;
            public int y;
            public int z;
            public int r;


            public int distancefrom(bot bot)
            {
                return Math.Abs(this.x - bot.x) + Math.Abs(this.y - bot.y) + Math.Abs(this.z - bot.z);
            }

            public bool inRangeFromMe(bot bot)
            {
                return distancefrom(bot) <= r;
            }
        }

        string testInput =
            @"pos=<0,0,0>, r=4
pos=<1,0,0>, r=1
pos=<4,0,0>, r=3
pos=<0,2,0>, r=1
pos=<0,5,0>, r=3
pos=<0,0,3>, r=1
pos=<1,1,1>, r=1
pos=<1,1,2>, r=1
pos=<1,3,1>, r=1";
    }

}
