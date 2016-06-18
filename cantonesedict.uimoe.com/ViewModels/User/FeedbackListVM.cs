using Agile.Dtos;
using Agile.Dtos.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.User
{
    public class FeedbackListVM
    {
        public PagedListDto<FeedbackListItemVM> Data { get; set; }
    }

    public class FeedbackListItemVM
    {
        public string ChnText { get; set; }

        public string CanText { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }

        public string StatusDisplay
        {
            get
            {
                if (!Status.HasValue)
                {
                    return "";
                }

                var statusDisplay = "";
                switch (Status.Value)
                {
                    case (int)H10022ResponseFeedbackStatusEnum.Normal:
                        {
                            statusDisplay = "已处理";
                        }
                        break;
                    case (int)H10022ResponseFeedbackStatusEnum.Pending:
                        {
                            statusDisplay = "处理中";
                        }
                        break;
                }

                return statusDisplay;
            }
        }
    }
}