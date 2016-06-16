using System.Collections.Generic;

namespace cantonesedict.uimoe.com.ViewModels
{
    public class UserInfoVM
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public List<UserPermissionVM> UserPermissions { get; set; }
    }

    public class UserPermissionVM
    {
        public string Name { get; set; }

        public string RawUrl { get; set; }
    }
}