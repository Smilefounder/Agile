using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class HBaseResponse
    {
        public long time { get; set; }

        public int error { get; set; }

        public string message { get; set; }

        public static HBaseResponse Succeed
        {
            get
            {
                return new HBaseResponse
                {
                    error = 0
                };
            }
        }

        public static HBaseResponse Faild
        {
            get
            {
                return new HBaseResponse
                {
                    error = 1,
                    message = "操作失败，请稍后再试"
                };
            }
        }
    }
}
