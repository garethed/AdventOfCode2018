using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    class Day3 : Day
    {

        public override void Test()
        {
            Utils.Test(Part1, "#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2", "4" );
        }

        private IEnumerable<Claim> claims;
        private int[] claimed;
        private int xmax;
        private int ymax;


        public override string Part1(string input, dynamic options)
        {
            PopulateClaims((string)input);

            /*            
            int i = 0;

            foreach (var c in claimed)
            {
                if (i % xmax == 0)
                {
                    Console.WriteLine();
                }
                Console.Write(c);
                i++;
            }*/


            return claimed.Count(c => c > 1).ToString();
        }

        private void PopulateClaims(string input)
        {
            claims = Utils.splitLines((string)input).Select(s => new Claim(s));

            xmax = claims.Max(c => c.dimensions.Right) + 1;
            ymax = claims.Max(c => c.dimensions.Bottom) + 1;

            claimed = new int[xmax * ymax];



            foreach (var claim in claims)
            {
                for (int x = claim.dimensions.X; x <= claim.dimensions.Right; x++)
                {
                    for (int y = claim.dimensions.Y; y <= claim.dimensions.Bottom; y++)
                    {
                        claimed[x + xmax * y]++;
                    }
                }
            }
        }


        public override string Part2(string input, dynamic options)
        {
            PopulateClaims((string)input);

            foreach (var claim in claims)
            {
                int overlaps = 0;

                for (int x = claim.dimensions.X; x <= claim.dimensions.Right; x++)
                {
                    for (int y = claim.dimensions.Y; y <= claim.dimensions.Bottom; y++)
                    {
                        overlaps += (claimed[x + xmax * y] - 1);
                    }

                }

                if (overlaps == 0)
                {
                    return claim.id.ToString();
                }

            }

            return null;
        }

        private class Claim
        {
            public int id;
            public Rectangle dimensions;

            public Claim (string description)
            {
                description = Regex.Replace(description, @"[^\d]+", " ").Trim();
                var parts = description.Split(' ').Select(t => int.Parse(t)).ToList();

                id = parts[0];
                dimensions = new Rectangle(parts[1], parts[2], parts[3] - 1, parts[4] - 1);
            }
        }
       
    }

}
