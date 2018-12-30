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
    class Day22 : Day
    {
        public override bool Test()
        {
            return Utils.Test(Part1, "10,10", "114", new { depth = 510 })
                && Utils.Test(Part2, "10,10", "45", new { depth = 510 });
        }

        public override string Input => "9,751";
        public override dynamic Options => new { depth = 11817 };

        public int[,] map;
        int totalrisk = 0;

        public override string Part1(string input, dynamic options)
        {
            int depth = options.depth;
            xmax = tx = int.Parse(input.Substring(0, input.IndexOf(",")));
            ymax = ty = int.Parse(input.Substring(input.IndexOf(",") + 1));

           

            buildMap(depth);

            return totalrisk.ToString();

        }

        void buildMap(int depth)
        {
            int[,] erosionlevels = new int[xmax + 1, ymax + 1];
            map = new int[xmax + 1, ymax + 1];

            for (int x = 0; x <= xmax; x++)
            {
                var el = erosionLevel(x, 16807, depth);
                erosionlevels[x, 0] = el;
                totalrisk += el % 3;
                map[x, 0] = el % 3;
            }

            for (int y = 0; y <= ymax; y++)
            {
                var el = erosionLevel(48271, y, depth);
                erosionlevels[0, y] = el;
                totalrisk += el % 3;
                map[0, y] = el % 3;
            }


            for (int i = 1; i <= Math.Min(xmax, ymax); i++)
            {
                for (int x = i; x <= xmax; x++)
                {                    
                    var el = (x == tx && i == ty) ? erosionLevel(0,0,depth) : erosionLevel(erosionlevels[x - 1, i], erosionlevels[x, i - 1], depth);
                    erosionlevels[x, i] = el;
                    totalrisk += el % 3;
                    map[x, i] = el % 3;
                }

                for (int y = i + 1; y <= ymax; y++)
                {
                    var el = (i == tx && y == ty) ? erosionLevel(0, 0, depth) : erosionLevel(erosionlevels[i - 1, y], erosionlevels[i, y - 1], depth);
                    erosionlevels[i, y] = el;
                    totalrisk += el % 3;
                    map[i, y] = el % 3;
                }

            }

            totalrisk -= (erosionlevels[xmax, ymax] % 3);
        }

        private int erosionLevel(int x, int y, int d)
        {
            var gl = x * y;
            var el = (gl + d) % 20183;
            return el;
        }

        int tx, ty, xmax, ymax;

        public override string Part2(string input, dynamic options)
        {
            int depth = options.depth;
            tx = int.Parse(input.Substring(0, input.IndexOf(",")));
            ty = int.Parse(input.Substring(input.IndexOf(",") + 1));
            xmax = Math.Max(tx, ty) * 4 / 3;
            ymax = xmax;

            buildMap(depth);


            int[,,] routecosts = new int[xmax + 3, ymax + 3, 3];

            routecosts[1, 1, 0] = 8;
            routecosts[1, 1, 1] = 1;
            routecosts[1, 1, 2] = 8;

            for (int x = 0; x <= xmax; x++)
            {
                for (int y = 0; y <= ymax; y++)
                {
                    var forbidden = map[x, y];
                    routecosts[x + 1, y + 1, forbidden] = int.MaxValue;
                }
            }

             int currentcost = 1;

            Console.WriteLine();

            try
            {
                while (true)
                {
                    currentcost++;
                    Utils.WriteTransient(currentcost.ToString());
                    var sametoolcost = currentcost - 1;
                    var changetoolcost = currentcost > 7 ? currentcost - 7 : -1;
                    for (int x = 0; x <= Math.Min(xmax, currentcost); x++)
                    {
                        for (int y = 0; y <= Math.Min(ymax, currentcost); y++)
                        {
                            for (int t = 0; t < 3; t++)
                            {
                                if (routecosts[x + 1, y + 1, t] == changetoolcost)
                                {
                                    setCost(routecosts, x + 1, y + 1, (t + 1) % 3, currentcost);
                                    setCost(routecosts, x + 1, y + 1, (t + 2) % 3, currentcost);
                                }
                                else if (routecosts[x + 1, y + 1, t] == sametoolcost)
                                {
                                    setCost(routecosts, x, y + 1, t, currentcost);
                                    setCost(routecosts, x + 1, y, t, currentcost);
                                    setCost(routecosts, x + 2, y + 1, t, currentcost);
                                    setCost(routecosts, x + 1, y + 2, t, currentcost);
                                }
                            }

                        }

                    }

                }
            }
            catch (CompleteException c)
            {
                return c.result;
            }


        }

        private void setCost(int[,,] routecosts, int x, int y, int t, int cost)
        {
            if (routecosts[x,y,t] == 0)
            {
                routecosts[x, y, t] = cost;
                //Console.WriteLine((x - 1) + ", " + (y - 1) + " (" + t + ") -> " + (cost - 1));
                if (x - 1 == tx && y - 1 == ty && t == 1)
                {
                    throw new CompleteException() { result = (cost - 1).ToString() };
                }
            }
        }

        public class CompleteException : Exception
        {
            public string result;
        }

        public string Part2x(string input, dynamic options)
        {
            int depth = options.depth;
            tx = int.Parse(input.Substring(0, input.IndexOf(",")));
            ty = int.Parse(input.Substring(input.IndexOf(",") + 1));
            xmax = Math.Max(tx, ty) * 4 / 3;
            ymax = xmax;

            buildMap(depth);

            cheapestPath = int.MaxValue;
            Stack<step> path = new Stack<step>();
            HashSet<int> used = new HashSet<int>();

            path.Push(new step() { x = 0, y = 0, tool = 1, cost = 0 });
            used.Add(0);

            extendPath(path, used, -1);

            return cheapestPath.ToString();
        }

        private void extendPath(Stack<step> path, HashSet<int> used, int prev)
        {
            var current = path.Peek();

            
            var dx = tx - current.x;
            var dy = ty - current.y;

            var cheapestpossible = current.cost + Math.Abs(dx) + Math.Abs(dy) + (current.tool == 1 ? 0 : 7);

            if (cheapestpossible >= cheapestPath)
            {
                return;
            }


            if (dy == 0 && dx == 0)
            {                
                cheapestPath = current.cost + (current.tool == 1 ? 0 : 7);
                Console.Write(cheapestPath + ": ");

                foreach (var step in path.Reverse())
                {
                   // Console.Write(step.x + ", " + step.y + " (" + step.tool + ") ->");
                }
                Console.WriteLine("");

                return;
            }

            var steps = new step[4];

            var px = new step() { x = current.x + 1, y = current.y, tool = current.tool, cost = current.cost + 1 };
            var py = new step() { x = current.x, y = current.y + 1, tool = current.tool, cost = current.cost + 1 };
            var mx = new step() { x = current.x - 1, y = current.y, tool = current.tool, cost = current.cost + 1 };
            var my = new step() { x = current.x, y = current.y - 1, tool = current.tool, cost = current.cost + 1 };

            var xfirst = Math.Abs(dx) > Math.Abs(dy);
            var xpos = dx > 0;
            var ypos = dy > 0;

            steps[0] = xfirst ? (xpos ? px : mx) : (ypos ? py : my);
            steps[1] = xfirst ? (ypos ? py : my) : (xpos ? px : mx);
            steps[2] = xfirst ? (ypos ? my : py) : (xpos ? mx : px);
            steps[3] = xfirst ? (xpos ? mx : px) : (ypos ? my : py);

            foreach (var step in steps)
            {
                if (step.x >= 0 && step.x <= xmax && step.y >= 0 && step.y <= ymax && step.id != prev /*&& !used.Contains(step.id)*/)
                {
                    if (map[step.x, step.y] == current.tool)
                    {
                        step.cost += 7;
                        step.changetool = true;
                    }
                }
                else
                {
                    step.cost = int.MaxValue;

                }
            }


            if (steps[0].changetool && !steps[1].changetool)
            {
                var s = steps[0];
                steps[0] = steps[1];
                steps[1] = s;
            }

            if (steps[2].changetool && !steps[3].changetool)
            {
                var s = steps[2];
                steps[2] = steps[3];
                steps[3] = s;
            }

            foreach (var step in steps)
            {
                if (step.cost < int.MaxValue)
                {
                    if (step.changetool)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            step.tool = (step.tool + 1) % 3;

                            if (step.tool != map[current.x, current.y])
                            {

                                path.Push(step);
                                used.Add(step.id);
                                extendPath(path, used, current.id);
                                path.Pop();
                                used.Remove(step.id);
                            }
                        }
                    }
                    else
                    {
                        path.Push(step);
                        used.Add(step.id);
                        extendPath(path, used, current.id);
                        path.Pop();
                        used.Remove(step.id);
                    }
                }
            }
        }

        int cheapestPath = int.MaxValue;

        public class step
        {
            public int x;
            public int y;
            public int tool;
            public int cost;
            public bool changetool;
            public int id => 10000 * x + y;
        }
    }

}
