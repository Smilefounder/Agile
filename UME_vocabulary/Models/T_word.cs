using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME_vocabulary.Models
{
    public class T_word
    {
        public string Text { get; set; }

        public List<T_translation> Translates { get; set; }

        public override string ToString()
        {
            var sb = "";
            if (Translates != null && Translates.Any())
            {
                foreach (var t in Translates)
                {
                    sb += string.Format("{0}.{1}; ", t.WordType, t.Text);
                }
            }

            return Text + " " + sb;
        }
    }

    public class T_translation
    {
        public string WordType { get; set; }

        public string Text { get; set; }
    }
}
