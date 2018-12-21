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
    class Day16 : Day
    {
        private bool debug = false;

        public override bool Test()
        {
            return Utils.Test(Part1, testInput, "1"); /*&&
            Utils.Test(Part2, testInputs, new string[] { "29064", "31284", "3478", "6474", "1140" });*/

        }


        public override string Part1(string input, dynamic options)
        {
            var data = RegexDeserializable.Deserialize<TestResult>(input);

            return data.Count(t => AllOps.Count(o => t.Evaluate(o)) >= 3).ToString();


        }

        public override string Part2(string input, dynamic options)
        {
            var data = RegexDeserializable.Deserialize<TestResult>(input);

            Dictionary<int, Op> opcodes = new Dictionary<int, Op>();

            var opcodeMap = data.Select(d => new { d.opcode, possibleOps = AllOps.Where(o => d.Evaluate(o)) });
            var groupedMap = opcodeMap.GroupBy(o => o.opcode).ToDictionary(g => g.Key, g => g.First().possibleOps.Where(o1 => g.All(o2 => o2.possibleOps.Contains(o1))));

            try
            {       
                findMatches(opcodes, groupedMap);
            }
            catch (CompleteException)
            {

            }

            
            foreach (var test in data)
            {
                var op = opcodes[test.opcode];
                Debug.Assert(test.Evaluate(op));
            }

            var input2 = Inputs.ForDay(162);
            var program = RegexDeserializable.Deserialize<Statement>(input2);

            var registers = new[] { 0, 0, 0, 0 };

            foreach (var statement in program)
            {
                var op = opcodes[statement.opcode];
                op.Apply(statement.p0, statement.p1, statement.p2, registers);
            }

            return registers[0].ToString();

        }

        List<Dictionary<int, Op>> allsolutions = new List<Dictionary<int, Op>>();

        private void findMatches(Dictionary<int, Op> opcodes, Dictionary<int,IEnumerable<Op>> data)
        {
            var allunused = data.Where(d => !opcodes.ContainsKey(d.Key)).Select(d => new { opcode = d.Key, possibleOps = d.Value.Where(o => !opcodes.ContainsValue(o)) }).OrderBy(d => d.possibleOps.Count());
            
            var unused = allunused.FirstOrDefault();

            if (unused != null)
            {
                foreach (var possibleOp in unused.possibleOps)
                {
                    opcodes[unused.opcode] = possibleOp;
                    findMatches(opcodes, data);
                }

                opcodes.Remove(unused.opcode);

            }
            else
            {
                var copy = new Dictionary<int, Op>(opcodes);

                /*if (!allsolutions.Any(s => s.OrderBy(k => k.Key).SequenceEqual(copy.OrderBy(k => k.Key))))
                {
                    allsolutions.Add(copy);
                    foreach (var kv in copy)
                    {
                        Console.Write(kv.Key + ":" + kv.Value.name + " ");
                    }
                    Console.WriteLine();
                }*/

                throw new CompleteException();
            }

        }

        private Op[] AllOps = new[]
        {
            new Op("addr", (a,b) => a+b),
            new Op("addi", (a,b) => a+b),
            new Op("mulr", (a,b) => a*b),
            new Op("muli", (a,b) => a*b),
            new Op("banr", (a,b) => a&b),
            new Op("bani", (a,b) => a&b),
            new Op("borr", (a,b) => a|b),
            new Op("bori", (a,b) => a|b),
            new Op("setr", (a,b) => a),
            new Op("seii", (a,b) => a),
            new Op("gtir", (a,b) => a > b ? 1 : 0),
            new Op("gtrr", (a,b) => a > b ? 1 : 0),
            new Op("gtri", (a,b) => a > b ? 1 : 0),
            new Op("eqir", (a,b) => a == b ? 1 : 0),
            new Op("eqrr", (a,b) => a == b ? 1 : 0),
            new Op("eqri", (a,b) => a == b ? 1 : 0),
        };

        [RegexDeserializable(@"(?<opcode>\d+) (?<p0>\d+) (?<p1>\d+) (?<p2>\d+)")]
        public class Statement
        {
            public int opcode;
            public int p0;
            public int p1;
            public int p2;
        }

        [RegexDeserializable(@"Before: \[(?<r0>\d+), (?<r1>\d+), (?<r2>\d+), (?<r3>\d+)]\r?\n(?<opcode>\d+) (?<p0>\d+) (?<p1>\d+) (?<p2>\d+)\r?\nAfter:  \[(?<a0>\d+), (?<a1>\d+), (?<a2>\d+), (?<a3>\d+)]")]
        public class TestResult
        {
            public int r0;
            public int r1;
            public int r2;
            public int r3;
            public int opcode;
            public int p0;
            public int p1;
            public int p2;
            public int a0;
            public int a1;
            public int a2;
            public int a3;

            public bool Evaluate(Op op)
            {
                int[] registers = new[] { r0, r1, r2, r3 };
                op.Apply(p0, p1, p2, registers);

                return registers.SequenceEqual(new[] { a0, a1, a2, a3 });

            }
        }

        public class Op
        {
            private Func<int, int, int> fn;
            private bool r1;
            private bool r2;
            public string name;

            public Op(string name, Func<int, int, int> fn)
            {
                this.name = name;
                this.fn = fn;
                r1 = name[2] != 'i';
                r2 = name[3] != 'i';            
            }

            public void Apply(int a1, int a2, int a3, int[] registers)
            {
                a1 = r1 ? registers[a1] : a1;
                a2 = r2 ? registers[a2] : a2;
                registers[a3] = fn(a1, a2);
            }
        }


        private string testInput =
@"Before: [3, 2, 1, 1]
9 2 1 2
After:  [3, 2, 2, 1]";

        [Serializable]
        private class CompleteException : Exception
        {
            public CompleteException()
            {
            }

            public CompleteException(string message) : base(message)
            {
            }

            public CompleteException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected CompleteException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

}
