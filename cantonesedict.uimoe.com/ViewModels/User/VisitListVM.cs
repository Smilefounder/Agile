using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.User
{
    public class VisitListVM
    {
        public PagedListDto<VisitListItemVM> data { get; set; }
    }

    public class VisitListItemVM
    {
        public string rawurl { get; set; }

        public string useragent { get; set; }

        public string ipaddress { get; set; }

        public string createdatstr { get; set; }
    }
}