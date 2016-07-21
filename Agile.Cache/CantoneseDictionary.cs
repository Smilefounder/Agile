using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Cache
{
    public class CantoneseDictionary
    {
        public static string FindAll(string keys)
        {
            var sb = new StringBuilder();
            foreach (var ch in keys)
            {
                var chstr = ch.ToString();
                if (String.IsNullOrEmpty(chstr))
                {
                    sb.Append("");
                    continue;
                }

                if (chstr.Trim().Length == 0)
                {
                    sb.Append(" ");
                    continue;
                }

                var vstr = chstr;
                if (Instance.ContainsKey(chstr))
                {
                    vstr = Instance[chstr] + " ";
                }

                sb.AppendFormat("{0}", vstr);
            }

            return sb.ToString();
        }

        public static string FindOne(char key)
        {
            var keystr = key.ToString();
            if (Instance.ContainsKey(keystr))
            {
                var list = Instance[keystr];
                if (list != null && list.Any())
                {
                    return string.Join(",", list);
                }
            }

            return "";
        }

        private static Dictionary<string, List<string>> _instance = null;

        private static Dictionary<string, List<string>> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Dictionary<string, List<string>>();

                    var dictfile = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "CantoneseDict", "Dict.dct" });
                    if (!System.IO.File.Exists(dictfile))
                    {
                        return _instance;
                    }

                    var lines = System.IO.File.ReadAllLines(dictfile);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        var subparts = parts[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (subparts.Length == 0)
                        {
                            continue;
                        }

                        var key = parts[0];
                        if (_instance.ContainsKey(key))
                        {
                            _instance[key] = new List<string>();
                        }
                        else
                        {
                            _instance.Add(key, new List<string>());
                        }

                        foreach (var sp in subparts)
                        {
                            _instance[key].Add(sp);
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
