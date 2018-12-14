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
    class Day14 : Day
    {
        public override void Test()
        {
            Utils.Test(Part1, "9", "5158916779");
            Utils.Test(Part2, "51589", "9");
            Utils.Test(Part2, "01245", "5");
            Utils.Test(Part2, "92510", "18");
            Utils.Test(Part2, "59414", "2018");

        }

        public override string Input => "323081";

        public override string Part1(string input, dynamic options)
        {
            var rounds = int.Parse(input);

            var e1 = 0;
            var e2 = 1;

            List<int> recipes = new List<int> { 3, 7 };

            while (recipes.Count < rounds + 10)
            {
                var sum = recipes[e1] + recipes[e2];
                if (sum >= 10)
                {
                    recipes.Add(1);
                }
                recipes.Add(sum % 10);

                e1 = (e1 + 1 + recipes[e1]) % recipes.Count;
                e2 = (e2 + 1 + recipes[e2]) % recipes.Count;

                //print(recipes);
            }

            var result = "";
            for (int i = 0; i < 10; i++)
            {
                result += recipes[recipes.Count - 10 + i];
            }

            return result;
            
        }

        public void print(List<int> recipes)
        {
            Console.WriteLine();
            foreach (var i in recipes)
            {
                Console.Write(i + " ");
            }
        }

        public override string Part2(string input, dynamic options)
        {
            var e1 = 0;
            var e2 = 1;

            List<int> recipes = new List<int> { 3, 7 };

            int total = 0;
            int target = int.Parse(input);
            int maxdigits = (int)Math.Pow(10, input.Length);


            for (int r = 0; ; r++)
            {
                var sum = recipes[e1] + recipes[e2];
                if (sum >= 10)
                {
                    recipes.Add(1);
                    total = 10 * total + 1;
                }

                total = total % maxdigits;

                if (total == target)
                {
                    return (recipes.Count - input.Length).ToString();
                }

                recipes.Add(sum % 10);
                total = 10 * total + (sum % 10);

                e1 = (e1 + 1 + recipes[e1]) % recipes.Count;
                e2 = (e2 + 1 + recipes[e2]) % recipes.Count;

                total = total % maxdigits;

                if (total == target)
                {
                    return (recipes.Count - input.Length).ToString();
                }

                // 323081
                if (recipes[recipes.Count - 1] == 1 && recipes[recipes.Count - 2] == 8 && recipes[recipes.Count - 3] == 0 && recipes[recipes.Count - 4] == 3)
                {
                    var x = 4;
                }

                if (r % 10000 == 0)
                {
                    var x = 2;
                }


            }
        }
    }

}
