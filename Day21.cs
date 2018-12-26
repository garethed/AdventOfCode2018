﻿using System;
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
    class Day21 : Day
    {
        public override bool Test()
        {
            return true;
        }

        int r0 = 0;

        public override string Part1(string input, dynamic options)
        {
            var ipr = int.Parse(input.Substring(4,1));
            var program = RegexDeserializable.Deserialize<Statement>(input.Substring(7)).ToArray();

            var opcodes = AllOps.ToDictionary(o => o.name);
            opcodes["seti"] = opcodes["seii"];

            r0 = 10780777;

            var registers = new[] { r0, 255, 0, 0, 0, 0 };
            var ip = 0;

            HashSet<int> r2Values = new HashSet<int>();


            while (ip >= 0 && ip < program.Length)
            {

                var statement = program[ip];
                var op = opcodes[statement.opcode];

                if (ip == 28)
                {
                    Console.WriteLine(registers[2]);
                }
                //Console.WriteLine(ip.ToString("00") + " - " + string.Join(", ", registers.Select(r => r.ToString("0000"))) + " " + statement.opcode + " " + statement.p0 + " " + statement.p1 + " " + statement.p2);

                registers[ipr] = ip;
                op.Apply(statement.p0, statement.p1, statement.p2, registers);
                ip = registers[ipr];
                ip++;

            }

            return registers[0].ToString();

        }

        public override string Part2(string input, dynamic options)
        {
            int r0 = 0, r1 = 0, r2 = 0, r3 = 0, r4 = 0, r5 = 0;
            int lastr2 = 0;
            int i = 0;
            HashSet<int> r2Values = new HashSet<int>();

            Line6:
            //bori 2 65536 1    #6 r1 = r2 | 65536               b000000010000000000000000
            r1 = r2 | 65536;
            //seti 1250634 6 2  # r2 = 1250634                   b000100110001010101001010
            r2 = 1250634;
            Line8:
            //bani 1 255 4      #8 r4 = r1 & 255                 b000000000000000011111111
            r4 = r1 & 255;
            //addr 2 4 2        # r2 = r4 + r2
            r2 = r4 + r2;
            //bani 2 16777215 2 # r2 = r2 & 16777215             b111111111111111111111111
            r2 = r2 & 16777215;
            //muli 2 65899 2    # r2 = r2 * 65899                b000000010000000101101011
            r2 = r2 * 65899;
            //bani 2 16777215 2 # r2 = r2 & 16777215
            r2 = r2 & 16777215;

//gtir 256 1 4      # r4 = 1 if r1 > 256
//addr 4 5 5        # add r4 to r5
//addi 5 1 5        # add 1 to r5 => skip next
//seti 27 2 5       # set r5 = 27 => jump to 28
            if (256 > r1)
            {
                r4 = 1;
                goto Line28;
            }

            //seti 0 5 4        # set r4 to 0
            r4 = 0;
        Line18:
            //addi 4 1 3        #18 set r3 = r4 + 1
            r3 = r4 + 1;
            //muli 3 256 3      # multiply r3 by 256
            r3 *= 256;
//gtrr 3 1 3        # set r3 = 1 if r3 > r1            
//addr 3 5 5        # add r3 to r5 => skip
            if (r3 > r1)
            {
                r3 = 1;
                goto Line26;
            }
            r3 = 0;
            //addi 5 1 5        # add 1 to r5 => skip
            //seti 25 5 5       # set r5 = 25 => jump to 26
            //addi 4 1 4        # add 4 to r4
            r4 += 1;
            //seti 17 2 5       # set r5 = 17 => jump to 18
            goto Line18;

            Line26:
            //setr 4 0 1        #26 set r1 = r4
            r1 = r4;
            //seti 7 6 5        # set r5 = 7 => jump to 8
            goto Line8;
            Line28:
            //eqrr 2 0 4        #28 set r4 = 1 if r2 = r0 
            if (r2Values.Contains(r2))
            {
                return lastr2.ToString();
            }
            lastr2 = r2;
            r2Values.Add(r2);
            //Console.WriteLine(i++ + " " + r2);
            //addr 4 5 5        # add r4 to r5  !TARGET!
            //seti 5 7 5        # set r5 to 5 => jump to 6
            r4 = 0;
            goto Line6;
        }

        List<Dictionary<int, Op>> allsolutions = new List<Dictionary<int, Op>>();

        private void findMatches(Dictionary<int, Op> opcodes, Dictionary<int, IEnumerable<Op>> data)
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

        private Op[] AllOps = new Op[]
        {
            new Op("banr", (a,b) => a&b),
            new Op("muli", (a,b) => a*b),
            new Op("bori", (a,b) => a|b),
            new Op("setr", (a,b) => a),
            new Op("addi", (a,b) => a+b),
            new Op("eqrr", (a,b) => a == b ? 1 : 0),
            new Op("gtri", (a,b) => a > b ? 1 : 0),
            new Op("gtir", (a,b) => a > b ? 1 : 0),
            new Op("borr", (a,b) => a|b),
            new Op("eqri", (a,b) => a == b ? 1 : 0),
            new Op("bani", (a,b) => a&b),
            new Op("addr", (a,b) => a+b),
            new Op("eqir", (a,b) => a == b ? 1 : 0),
            new Op("mulr", (a,b) => a*b),
            new Op("seii", (a,b) => a),
            new Op("gtrr", (a,b) => a > b ? 1 : 0),
        };

        [RegexDeserializable(@"^(?<opcode>\w+) (?<p0>\d+) (?<p1>\d+) (?<p2>\d+)")]
        public class Statement
        {
            public string opcode;
            public int p0;
            public int p1;
            public int p2;
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
@"#ip 0
seti 5 0 1
seti 6 0 2
addi 0 1 0
addr 1 2 3
setr 1 0 0
seti 8 0 4
seti 9 0 5";

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

/*

03 - 0000, 0001, 0976, 0000, 0002, 0004 mulr 1 5 3 # mult r5 * r1 === 1, so move 5 to 3
04 - 0000, 0001, 0976, 0004, 0003, 0004 eqrr 3 2 3 # set 3 to zero iff eq r2
05 - 0000, 0001, 0976, 0000, 0004, 0004 addr 3 4 4 # add r3 to the ip, will jump to 7 when r3 is r2
06 - 0000, 0001, 0976, 0000, 0005, 0004 addi 4 1 4 # add 1 to r4, jump to 8
(07 addr 1 0 0)
08 - 0000, 0001, 0976, 0000, 0007, 0004 addi 5 1 5 # add 1 to r5
09 - 0000, 0001, 0976, 0000, 0008, 0005 gtrr 5 2 3 # if r5 > r2 then r3 = 1
10 - 0000, 0001, 0976, 0000, 0009, 0005 addr 4 3 4 # add r3 to ip - so jump to 12 iff r5 > r2
11 - 0000, 0001, 0976, 0000, 0010, 0005 seti 2 2 4 # set ip to 2
03 - 0000, 0001, 0976, 0000, 0002, 0005 mulr 1 5 3
04 - 0000, 0001, 0976, 0005, 0003, 0005 eqrr 3 2 3
05 - 0000, 0001, 0976, 0000, 0004, 0005 addr 3 4 4
06 - 0000, 0001, 0976, 0000, 0005, 0005 addi 4 1 4
08 - 0000, 0001, 0976, 0000, 0007, 0005 addi 5 1 5
09 - 0000, 0001, 0976, 0000, 0008, 0006 gtrr 5 2 3
10 - 0000, 0001, 0976, 0000, 0009, 0006 addr 4 3 4
11 - 0000, 0001, 0976, 0000, 0010, 0006 seti 2 2 4
03 - 0000, 0001, 0976, 0000, 0002, 0006 mulr 1 5 3
04 - 0000, 0001, 0976, 0006, 0003, 0006 eqrr 3 2 3
05 - 0000, 0001, 0976, 0000, 0004, 0006 addr 3 4 4
06 - 0000, 0001, 0976, 0000, 0005, 0006 addi 4 1 4
08 - 0000, 0001, 0976, 0000, 0007, 0006 addi 5 1 5
09 - 0000, 0001, 0976, 0000, 0008, 0007 gtrr 5 2 3
10 - 0000, 0001, 0976, 0000, 0009, 0007 addr 4 3 4
11 - 0000, 0001, 0976, 0000, 0010, 0007 seti 2 2 4


    */