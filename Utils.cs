using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    static class Utils
    {
        public static IEnumerable<string> splitLines(string input)
        {
            return input.Replace("\r", "").Split('\n').Select(l => l.Trim());
        }

        public static void Test(Func<dynamic, string> method, dynamic input, string output)
        {
            Test(method, new dynamic[] { input }, new string[] { output });
        }

        public static void Test(Func<dynamic, string> method, dynamic[] inputs, string[] outputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                var actual = method(inputs[i]);
                if (actual == outputs[i])
                {
                    Write("OK: ", ConsoleColor.Green);
                }
                else
                {
                    Write("WRONG: ", ConsoleColor.Red);
                }

                var input = (string)inputs[i].Replace("\n", " ");
                if (input.Length > 20)
                {
                    input = input.Substring(0, 20) + "...";
                }

                Write(input + " -> ", ConsoleColor.White);

                if (actual != outputs[i])
                {
                    WriteLine(actual, ConsoleColor.Red);
                    WriteLine("  Should be " + outputs[i], ConsoleColor.Red);
                }
                else
                {
                    WriteLine(actual, ConsoleColor.Green);
                }
            }
        }

        static MD5 md5 = System.Security.Cryptography.MD5.Create();


        public static string MD5(string input)
        {
            return BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(input))).Replace("-", "").ToLower();
        }

        public static void WriteLine(string msg, ConsoleColor color)
        {
            Write(msg, color);
            Console.WriteLine();
        }


        public static void Write(string msg, ConsoleColor color)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ForegroundColor = old;
        }

        public static void ClearLine()
        {
            Console.Write(new string(' ', Console.WindowWidth));
            Console.CursorLeft = 0;
        }

        public static void WriteTransient(string msg)
        {
            Console.Write(msg);
            Console.CursorLeft = 0;
        }
    }
}
