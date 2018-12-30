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
    class Day25 : Day
    {
        public override bool Test()
        {
            return Utils.Test(Part1, testInputs, testOutputs);
        }

        public override string Part1(string input, dynamic options)
        {
            var points = RegexDeserializable.Deserialize<Point4D>(input).OrderBy(p => p.x).ThenBy(p => p.y).ThenBy(p => p.z).ThenBy(p => p.t).ToList();
            var groups = new List<HashSet<Point4D>>();

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                for (int j = i + 1; j < points.Count; j++)
                {
                    point.testNeighbour(points[j]);
                }
            }

            var unused = new HashSet<Point4D>(points);

            while (unused.Count > 0)
            {
                var point = unused.First();
                var group = buildGroup(point);
                unused.ExceptWith(group);
                groups.Add(group);
            }

            return groups.Count.ToString();
        }

        private HashSet<Point4D> buildGroup(Point4D point)
        {
            var group = new HashSet<Point4D>();
            addPointToGroup(group, point);
            return group;
        }

        private void addPointToGroup(HashSet<Point4D> group, Point4D point)
        {
            group.Add(point);
            foreach (var neighbour in point.neighbours)
            {
                if (!group.Contains(neighbour))
                {
                    addPointToGroup(group, neighbour);
                }
            }
        }

        public override string Part2(string input, dynamic options)
        {
            return null;

        }       

        [RegexDeserializable(@"(?<x>\-?\d+),(?<y>-?\d+),(?<z>-?\d+),(?<t>-?\d+)")]
        public class Point4D
        {
            public int x;
            public int y;
            public int z;
            public int t;

            public List<Point4D> neighbours = new List<Point4D>();

            public void testNeighbour(Point4D point)
            {
                var distance = Math.Abs(x - point.x) + Math.Abs(y - point.y) + Math.Abs(z - point.z) + Math.Abs(t - point.t);
                if (distance <= 3)
                {
                    this.neighbours.Add(point);
                    point.neighbours.Add(this);
                }
            }
        }

        string[] testInputs = new[]
        {
            @" 0,0,0,0
 3,0,0,0
 0,3,0,0
 0,0,3,0
 0,0,0,3
 0,0,0,6
 9,0,0,0
12,0,0,0",
            @"-1,2,2,0
0,0,2,-2
0,0,0,-2
-1,2,0,0
-2,-2,-2,2
3,0,2,-1
-1,3,2,2
-1,0,-1,0
0,2,1,-2
3,0,0,0",
            @"1,-1,0,1
2,0,-1,0
3,2,-1,0
0,0,3,1
0,0,-1,-1
2,3,-2,0
-2,2,0,0
2,-2,0,-1
1,-1,0,-1
3,2,0,2",
            @"1,-1,-1,-2
-2,-2,0,1
0,2,1,3
-2,3,-2,1
0,2,3,-2
-1,-1,1,-2
0,-2,-1,0
-2,2,3,-1
1,2,2,0
-1,-2,0,-2"
        };

        string[] testOutputs = new[] { "2", "4", "3", "8" };
            
    }

}
