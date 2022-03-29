﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace discosh.Protections
{
    public class StringSplit
    {
        public static string GenCode(string input, Random rng, int level = 5)
        {
            string ret = string.Empty;
            string[] lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int amount = 5;
            if (level > 1) amount -= level;
            amount *= 2;

            List<string> setlines = new List<string>();
            List<string[]> linevars = new List<string[]>();
            foreach (string line in lines)
            {
                List<string> splitted = new List<string>();
                string sc = string.Empty;
                bool invar = false;
                foreach (char c in line)
                {
                    if (c == '%')
                    {
                        invar = !invar;
                        sc += c;
                        continue;
                    }
                    if (c == ' ' && invar)
                    {
                        invar = false;
                        sc += c;
                        continue;
                    }
                    if (!invar)
                    {
                        if (sc.Length >= amount)
                        {
                            splitted.Add(sc);
                            invar = false;
                            sc = string.Empty;
                        }
                    }
                    sc += c;
                }
                splitted.Add(sc);

                List<string> vars = new List<string>();
                foreach (string s in splitted)
                {
                    string name = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "");
                    name = new string(name.Where(char.IsLetter).ToArray());
                    name = name.Substring(0, 10);

                    setlines.Add($"set \"{name}={s}\"");
                    vars.Add(name);
                }
                linevars.Add(vars.ToArray());
            }

            setlines = new List<string>(setlines.OrderBy(x => rng.Next()));
            for (int i = 0; i < setlines.Count; i++) // Write all variables in random order
            {
                ret += setlines[i];
                int r = rng.Next(0, 2);
                if (r == 0 || i == setlines.Count - 1) ret += Environment.NewLine;
                else ret += " && ";
            }

            foreach (string[] line in linevars)
            {
                foreach (string s in line) ret += $"%{s}%";
                ret += Environment.NewLine;
            }

            return ret;
        }
    }
}
