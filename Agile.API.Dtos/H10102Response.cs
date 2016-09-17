using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10102Response : HBaseResponse
    {
        public PagedListDto<H10102ResponseListItem> data { get; set; }
    }

    public class H10102ResponseListItem
    {
        public string name { get; set; }

        public int source { get; set; }

        public string sourcedisplay
        {
            get
            {
                var display = "";
                switch (source)
                {
                    case (int)H10102ResponseListItemSourceEnum.Alipay:
                        {
                            display = "支付宝";
                        }
                        break;
                    case (int)H10102ResponseListItemSourceEnum.Tenpay:
                        {
                            display = "微信支付";
                        }
                        break;
                }

                return display;
            }
        }

        public decimal money { get; set; }

        public DateTime createdat { get; set; }
    }

    public enum H10102ResponseListItemSourceEnum
    {
        Alipay = 0,

        Tenpay = 1
    }
}
