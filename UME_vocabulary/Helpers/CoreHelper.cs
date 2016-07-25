using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UME_vocabulary.Helpers
{
    public class CoreHelper
    {
        static CoreHelper()
        {
            Vocabulary = new List<string>();
            Remembered = new List<string>();
        }

        private static List<string> Vocabulary { get; set; }

        private static List<string> Remembered { get; set; }

        private static List<string> NewWords
        {
            get
            {
                if (Remembered.Count == 0)
                {
                    return Vocabulary;
                }

                var list = new List<string>();
                foreach (var v in Vocabulary)
                {
                    var cnt = Remembered.Count(c => c == v);
                    if (cnt == 0)
                    {
                        list.Add(v);
                    }
                }

                return list;
            }
        }

        public static void RememberOne(string model)
        {
            Remembered.Add(model);
        }

        public static string NextOne()
        {
            var num = NewWords.Count;
            if (num == 0)
            {
                return null;
            }

            var r = new Random();
            var idx = r.Next(0, num);
            return NewWords[idx];
        }

        public static string GetRememberedStr()
        {
            if (Remembered != null && Remembered.Any())
            {
                var sb = new StringBuilder();
                foreach (var r in Remembered)
                {
                    sb.AppendLine(r);
                }

                return sb.ToString();
            }

            return null;
        }

        public static void BuildVocabulary(params string[] lines)
        {
            if (lines != null && lines.Any())
            {
                foreach (var i in lines)
                {
                    var word = ParseWordFromStr(i);
                    if (!string.IsNullOrEmpty(word))
                    {
                        Vocabulary.Add(word);
                    }
                }
            }
        }

        private static string ParseWordFromStr(string line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Trim().Length < 4)
            {
                return null;
            }

            return line;
        }

        public static void BuildRemembered(params string[] lines)
        {
            if (lines != null && lines.Any())
            {
                foreach (var i in lines)
                {
                    Remembered.Add(i);
                }
            }
        }
    }
}
