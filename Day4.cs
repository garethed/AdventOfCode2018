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
    class Day4 : Day
    {

        public override void Test()
        {
            Utils.Test(Part1, TestData, "240" );
            Utils.Test(Part2, TestData, "4455");
        }

     
        public override string Part1(string input, dynamic options)
        {
            var sleep = ProcessSleepData((string)input);

            var sleepiestguard = sleep.OrderByDescending(kv => kv.Value[60]).First();
            var sleepiesttime = Array.IndexOf(sleepiestguard.Value, sleepiestguard.Value.Take(60).Max());

            return (sleepiestguard.Key * sleepiesttime).ToString();
        }

        public override string Part2(string input, dynamic options)
        {
            var sleep = ProcessSleepData((string)input);

            var sleepiestguard = sleep.OrderByDescending(kv => kv.Value.Take(60).Max()).First();
            var sleepiesttime = Array.IndexOf(sleepiestguard.Value, sleepiestguard.Value.Take(60).Max());

            return (sleepiestguard.Key * sleepiesttime).ToString();
        }

        private Dictionary<int, int[]> ProcessSleepData(string input)
        {
            var records = Utils.splitLines((string)input).OrderBy(l => l).Select(l => new EventRecord(l));

            int guardid = 0;
            bool awake = true;
            int sleeptime = 0;

            var sleep = new Dictionary<int, int[]>();

            foreach (var record in records)
            {
                switch (record.type)
                {
                    case EventType.onduty:
                        guardid = record.GuardId;
                        Debug.Assert(awake);
                        break;
                    case EventType.sleeps:
                        sleeptime = record.Minute;
                        break;
                    case EventType.wakes:
                        if (!sleep.ContainsKey(guardid))
                        {
                            sleep.Add(guardid, new int[61]);
                        }
                        for (int t = sleeptime; t < record.Minute; t++)
                        {
                            sleep[guardid][t]++;
                            sleep[guardid][60]++;
                        }
                        break;
                }

            }

            return sleep;
        }

        private class EventRecord
        {
            public int Minute;
            public int GuardId;
            public EventType type;

            public EventRecord (string description)
            {
                Minute = int.Parse(description.Substring(15, 2));

                if (description.Contains("asleep"))
                {
                    type = EventType.sleeps;
                }
                else if (description.Contains("wakes"))
                {
                    type = EventType.wakes;
                }
                else
                {
                    type = EventType.onduty;
                    GuardId = int.Parse(description.Split(' ')[3].Substring(1));
                }
            }
        }

        private enum EventType
        {
            sleeps,
            wakes,
            onduty
        }

        private string TestData =
@"[1518-11-01 00:00] Guard #10 begins shift
[1518-11-01 00:05] falls asleep
[1518-11-01 00:25] wakes up
[1518-11-01 00:30] falls asleep
[1518-11-01 00:55] wakes up
[1518-11-01 23:58] Guard #99 begins shift
[1518-11-02 00:40] falls asleep
[1518-11-02 00:50] wakes up
[1518-11-03 00:05] Guard #10 begins shift
[1518-11-03 00:24] falls asleep
[1518-11-03 00:29] wakes up
[1518-11-04 00:02] Guard #99 begins shift
[1518-11-04 00:36] falls asleep
[1518-11-04 00:46] wakes up
[1518-11-05 00:03] Guard #99 begins shift
[1518-11-05 00:45] falls asleep
[1518-11-05 00:55] wakes up";


    }

}
