using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UME_vocabulary.Models;

namespace UME_vocabulary.Helpers
{
    public class CoreHelper
    {
        static CoreHelper()
        {
            Vocabulary = new List<T_word>();
            Remembered = new List<string>();
        }

        private static List<T_word> Vocabulary { get; set; }

        private static List<string> Remembered { get; set; }

        public static void RememberOne(T_word model)
        {
            Remembered.Add(model.Text);
        }

        public static T_word NextOne()
        {
            var num = Vocabulary.Count;
            if (num == 0)
            {
                return null;
            }

            var r = new Random();
            var idx = r.Next(0, num);
            return Vocabulary[idx];
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
                    Vocabulary.Add(word);
                }
            }
        }

        private static T_word ParseWordFromStr(string line)
        {
            var parts = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return new T_word
                {
                    Text = parts[0]
                };
            }

            var word = new T_word
            {
                Text = parts[0],
                Translates = new List<T_translation>()
            };

            var subparts = parts[1].Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sp in subparts)
            {
                var subparts2 = sp.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (subparts2.Length != 2)
                {
                    continue;
                }

                word.Translates.Add(new T_translation
                {
                    Text = subparts2[1],
                    WordType = subparts2[0]
                });
            }

            return word;
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
