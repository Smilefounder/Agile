using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10100Response : HBaseResponse
    {
        public H10100ResponseListItem data { get; set; }
    }

    public class H10100ResponseListItem
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string TitleDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Title) || Title.Length < 12)
                {
                    return Title;
                }

                return Title.Substring(0, 12) + "...";
            }
        }

        public string Cover { get; set; }

        public decimal Price { get; set; }

        public decimal Rate { get; set; }
    }
}
