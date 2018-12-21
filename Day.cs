using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    abstract class Day
    {
        public abstract bool Test();
        public abstract string Part1(string input, dynamic options);
        public abstract string Part2(string input, dynamic options);
        //public abstract dynamic Input { get; }

        public int Index
        {
            get { return int.Parse(this.GetType().Name.Substring(3)); }
        }

        public virtual string Input => Inputs.ForDay(Index);

        public virtual dynamic Options => new object();
    }
}
