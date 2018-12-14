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
    class Day11 : Day
    {

        public override void Test()
        {
            Utils.Test(Part1, "18", "33,45");
            Utils.Test(Part2, "18", "90,269,16");
            Utils.Test(Part2, "42", "232,251,12");
            
            //Utils.Test(Part2, "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2", "66");
        }

        public override string Input => "7857";


        public override string Part1(string input, dynamic options)
        {
            var serialNo = int.Parse(input);
            var cells = new int[300, 300];

            for (int x = 1; x <= 300; x++)
            {
                for (int y = 1; y < 300; y++)
                {
                    var rackId = x + 10;
                    var power = rackId * y;
                    power += serialNo;
                    power *= rackId;
                    power = power % 1000;
                    power = power / 100;
                    power -= 5;

                    cells[x - 1, y - 1] = power;
                }
            }

            int tmax = 0, xmax = 0, ymax = 0;

            for (int x = 0; x < 300 - 3; x++)
            {
                for (int y = 0; y < 300 - 3; y++)
                {
                    var total = 0;
                    for (int dx = 0; dx < 3; dx++)
                    {
                        for (int dy = 0; dy < 3; dy++)
                        {
                            total += cells[x + dx, y + dy];
                        }
                    }

                    //cells[x, y] = total;                    

                    if (total > tmax)
                    {
                        tmax = total;
                        xmax = x;
                        ymax = y;
                    }
                }
            }

            return (xmax + 1) + "," + (ymax + 1);
        }

        

        public override string Part2(string input, dynamic options)
        {
            var serialNo = int.Parse(input);
            var cells = new int[300, 300];

            for (int x = 1; x <= 300; x++)
            {
                for (int y = 1; y < 300; y++)
                {
                    var rackId = x + 10;
                    var power = rackId * y;
                    power += serialNo;
                    power *= rackId;
                    power = power % 1000;
                    power = power / 100;
                    power -= 5;

                    cells[x - 1, y - 1] = power;
                }
            }

            int tmax = 0, xmax = 0, ymax = 0, nmax = 0;

            for (int x = 0; x < 300; x++)
            {
                for (int y = 0; y < 300; y++)
                {
                    var total = 0;

                    for (int n = 0; x + n < 300 && y + n < 300; n++)
                    {
                        total += cells[x + n, y + n];
                        for (int x2 = x; x2 < x + n; x2++)
                        {
                            total += cells[x2, y + n];
                        }
                        for (int y2 = y; y2 < y + n; y2++)
                        {
                            total += cells[x + n, y2];
                        }

                        if (total > tmax)
                        {
                            tmax = total;
                            xmax = x;
                            ymax = y;
                            nmax = n;
                        }

                    }            
                }
            }

            return (xmax + 1) + "," + (ymax + 1) + "," + (nmax + 1);
        }        
    }

}
