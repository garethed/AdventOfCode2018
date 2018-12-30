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
    class Day24 : Day
    {
        public override bool Test()
        {
            return Utils.Test(Part1, testInput, "5216") && Utils.Test(Part2, testInput, "51");
        }

        public override string Part1(string input, dynamic options)
        {
            var parts = input.Split(':');
            var immune = RegexDeserializable.Deserialize<UnitGroup>(parts[1]);
            var infection = RegexDeserializable.Deserialize<UnitGroup>(parts[2]);

            immune.ForEach(u => u.faction = "immune");
            infection.ForEach(u => u.faction = "infection");

            var units = new List<UnitGroup>();
            units.AddRange(infection);
            units.AddRange(immune);



            return battle(infection, immune, 0).Item2.ToString();
        }

        public override string Part2(string input, dynamic options)
        {
            var parts = input.Split(':');
            var immune = RegexDeserializable.Deserialize<UnitGroup>(parts[1]);
            var infection = RegexDeserializable.Deserialize<UnitGroup>(parts[2]);

            immune.ForEach(u => u.faction = "immune");
            infection.ForEach(u => u.faction = "infection");

            int lower = 0;
            int upper = 128;

            while (battle(new List<UnitGroup>(infection), new List<UnitGroup>(immune), upper).Item1 == "infection")
            {
                upper *= 2;
            }

            while (upper - lower > 1)
            {
                var mid = (upper + lower) / 2;
                var winner = battle(infection, immune, mid).Item1;
                if (winner == "infection")
                {
                    lower = mid;
                }
                else
                {
                    upper = mid;
                }
                Utils.WriteLine(lower + "-" + upper, ConsoleColor.Gray);
            }

            return battle(infection, immune, upper).Item2.ToString();

        }

        private Tuple<string, int> battle(List<UnitGroup> infection, List<UnitGroup> immune, int boost)
        {
            immune.ForEach(g => g.Reset(boost));
            infection.ForEach(g => g.Reset(0));

            while (immune.Any(g => g.count > 0) && infection.Any(g => g.count > 0))
            {
                //Console.WriteLine("New round");
                targetSelections.Clear();
                selectTargets(infection, immune);
                selectTargets(immune, infection);
                if (!attack())
                {
                    return Tuple.Create("infection", 0);
                }
            }

            if (immune.Any(g => g.count > 0))
            {
                return Tuple.Create("immune", immune.Sum(g => g.count));
            }
            else
            {
                return Tuple.Create("infection", infection.Sum(g => g.count));
            } 
        }

        private bool attack()
        {
            int totalKilled = 0;

            foreach (var kv in targetSelections.OrderByDescending(kv => kv.Key.initiative))
            {
                var target = kv.Value;
                var damage = kv.Key.PotentialDamage(target);
                var unitsKilled = Math.Min(target.count, damage / target.hitpoints);
                target.count -= unitsKilled;
                totalKilled += unitsKilled;
                //Utils.WriteLine(kv.Key + " killed " + unitsKilled + " from " + target, ConsoleColor.Gray);
            }

            return totalKilled > 0;
            
        }

        Dictionary<UnitGroup, UnitGroup> targetSelections = new Dictionary<UnitGroup, UnitGroup>();

        private void selectTargets(List<UnitGroup> force1, List<UnitGroup> force2)
        {
            var targets = new List<UnitGroup>(force2.Where(t => t.count > 0));

            foreach (var group in force1.OrderByDescending(g => g.power).ThenByDescending(g => g.initiative))
            {
                var possibleTarget = targets.OrderByDescending(t => group.PotentialDamage(t)).ThenByDescending(t => t.power).ThenByDescending(t => t.initiative).FirstOrDefault();
                if (possibleTarget != null && group.PotentialDamage(possibleTarget) > 0)
                {
                    targetSelections[group] = possibleTarget;
                    targets.Remove(possibleTarget);
                }
            }
        }



        [RegexDeserializable(@"(?<originalCount>\d+) units each with (?<hitpoints>\d+) hit points .*?$")]
        public class UnitGroup
        {
            public int originalCount;
            [RegexDeserializable(@"that does (\d+)")]
            public int originalAttackPower;

            public string faction;
            public int count;
            public int hitpoints;
            [RegexDeserializable("weak to ([^);]+)")]
            public string weaknesses = "";
            [RegexDeserializable("immune to ([^);]+)")]
            public string immunities = "";
            public int attackpower;
            [RegexDeserializable(@"\d (\w+) damage")]
            public string attacktype;
            [RegexDeserializable(@"initiative (\d+)")]
            public int initiative;

            public void Reset(int boost)
            {
                count = originalCount;
                attackpower = originalAttackPower + boost;
            }

            public int power => count * attackpower;

            internal int PotentialDamage(UnitGroup target)
            {
                var damage = count * attackpower;

                if (target.weaknesses.Contains(attacktype))
                {
                    damage *= 2;
                }
                else if (target.immunities.Contains(attacktype))
                {
                    damage = 0;
                }

                return damage;
            }

            public override string ToString()
            {
                return faction + " with atk " + attackpower;
            }
        }

        string testInput =
            @"Immune System:
17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

Infection:
801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4";
    }

}
