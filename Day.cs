using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    abstract class Day
    {
        public abstract void Test();
        public abstract string Part1(dynamic input);
        public abstract string Part2(dynamic input);
        //public abstract dynamic Input { get; }

        public int Index
        {
            get { return int.Parse(this.GetType().Name.Substring(3)); }
        }

        public string Input => Inputs.ForDay(Index);

    }
}
