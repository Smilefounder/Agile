using Agile.Dtos;
using Agile.Dtos.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.User
{
    public class UserListVM
    {
        public PagedListDto<UserListItemVM> data { get; set; }
    }

    public class UserListItemVM
    {
        public string UserName { get; set; }

        public int? Status { get; set; }

        public string StatusDisplay
        {
            get
            {
                if (!Status.HasValue)
                {
                    return "";
                }

                var display = "";

                switch (Status.Value)
                {
                    case (int)H10047UserStatusEnum.Normal:
                        {
                            display = "正常";
                        }
                        break;
                    case (int)H10047UserStatusEnum.Pending:
                        {
                            display = "待审核";
                        }
                        break;
                    case (int)H10047UserStatusEnum.Forbidden:
                        {
                            display = "已禁止";
                        }
                        break;
                }

                return display;
            }
        }
    }
}