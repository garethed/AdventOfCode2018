using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace AdventOfCode2018
{

    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class RegexDeserializable : System.Attribute
    {
        private string regex;

        public RegexDeserializable(string regex)
        {
            this.regex = regex;
        }

        public static List<T> Deserialize<T>(string serialized)
        {
            RegexDeserializable[] attributes = (RegexDeserializable[]) typeof(T).GetCustomAttributes(typeof(RegexDeserializable), false);
            var regex = attributes[0].regex;

            Regex r = new Regex(regex, RegexOptions.Singleline | RegexOptions.Multiline);
            List<T> results = new List<T>();

            var matches = r.Matches(serialized);

            foreach (Match match in matches)
            {
                var output = Activator.CreateInstance<T>();

                foreach (var groupName in r.GetGroupNames().Skip(1))
                {
                    FieldInfo fi = typeof(T).GetField(groupName);
                    string value = match.Groups[groupName].Value;
                    var converted = Convert.ChangeType(value, fi.FieldType);
                    fi.SetValue(output, converted);

                }

                results.Add(output);

            }

            return results;
        }
    }
}
