using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10064Response:HBaseResponse
    {
        public List<KeyValueDto> data { get; set; }
    }
}
