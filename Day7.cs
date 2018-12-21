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
    class Day7 : Day
    {
        SortedDictionary<char, Step> steps = new SortedDictionary<char, Step>();

        public override bool Test()
        {
            return Utils.Test(Part1, testInput, "CABDFE") &&
            Utils.Test(Part2, testInput, "15", new { basetime = 0, elves = 2 });
        }

        public override dynamic Options => new { basetime = 60, elves = 5 };

        public override string Part1(string input, dynamic options)
        {
            parseInput(input);

            string result = "";

            while (true)
            {
                var next = steps.Values.Where(s => !s.Complete && s.dependencies.Count == 0).FirstOrDefault();

                if (next == null)
                {
                    return result;
                }

                next.MarkComplete();
                result += next.ID;
            }
        }

        private void parseInput(string input)
        {
            var dependencies = Utils.splitLines((string)input).Select(l => (l[5], l[36]));
            steps.Clear();

            foreach (var pair in dependencies)
            {
                var step1 = GetStep(pair.Item1);
                var step2 = GetStep(pair.Item2);

                step2.dependencies.Add(step1);
                step1.dependants.Add(step2);
            }

        }

        private Step GetStep(char ID)
        {
            if (!steps.ContainsKey(ID))
            {
                steps.Add(ID, new Step() { ID = ID });
            }

            return steps[ID];
        }


        public override string Part2(string input, dynamic options)
        {
            parseInput(input);

            string output = "";

            for (int tick = 0; true; tick++)
            {
                foreach (var step in steps.Values)
                {
                    step.Tick(tick);
                    if (step.CompleteAtStartOfTick == tick)
                    {
                        output += step.ID;
                    }
                }

                var available = steps.Values.Where(s => !s.Complete && !s.InProgress(tick) && s.dependencies.Count == 0).ToList();
                var workers = options.elves - steps.Values.Where(s => s.InProgress(tick)).Count();

                for (int i = 0; i < workers && i < available.Count; i++)
                {
                    available[i].Start(tick, options.basetime);
                }

                if (steps.Values.All(s => s.Complete))
                {
                    return tick.ToString();
                }
            }
        }

        private string testInput =
@"Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.";

        private class Step
        {
            public char ID;
            public int Duration => (ID - 'A') + 1;
            public List<Step> dependencies = new List<Step>();
            public List<Step> dependants = new List<Step>();

            public bool Complete;
            public int CompleteAtStartOfTick = int.MaxValue;

            public void Tick(int currentTick)
            {
                if (currentTick == CompleteAtStartOfTick)
                {
                    MarkComplete();
                }
            }

            public void Start(int currentTick, int tickBase)
            {
                CompleteAtStartOfTick = currentTick + tickBase + Duration;
            }

            public bool InProgress(int currentTick)
            {
                return CompleteAtStartOfTick < int.MaxValue && CompleteAtStartOfTick > currentTick;
            }

            public void MarkComplete()
            {
                Complete = true;
                foreach (var step in dependants)
                {
                    step.dependencies.Remove(this);
                }
            }
        }
    }

}
