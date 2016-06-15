using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
    public class MeVM
    {
        public string UserName { get; set; }

        public List<UserPermissionVM> PermissionList { get; set; }
    }

    public class UserPermissionVM
    {
        public string Name { get; set; }

        public string RawUrl { get; set; }
    }
}