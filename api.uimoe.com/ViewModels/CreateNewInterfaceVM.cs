using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.uimoe.com.ViewModels
{
    public class CreateNewInterfaceVM
    {
        [DisplayName("编码")]
        public string Code { get; set; }

        [DisplayName("名称")]
        public string Name { get; set; }

        [DisplayName("描述")]
        public string Description { get; set; }
    }
}
