using Agile.Dtos;
using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using sharp.uimoe.com.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sharp.uimoe.com.Helpers
{
    public class CoreHelper
    {
        public static object Select(SelectDto dto)
        {
            var obj = new H10000Response
            {
                error = 1,
                data = new PagedListDto<H10000ResponseListItem>
                {
                    RecordList = new List<H10000ResponseListItem>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10000(new H10000Request
                {
                    page = dto.page,
                    pagesize = dto.pagesize
                });

                var response = responsebase as H10000Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null)
                {
                    obj = new H10000Response
                    {
                        error = 0,
                        data = new PagedListDto<H10000ResponseListItem>
                        {
                            Page = response.data.Page,
                            PageSize = response.data.PageSize,
                            RecordCount = response.data.RecordCount,
                            RecordList = response.data.RecordList

                        }
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return obj;
        }

        public static object Ul(UlDto dto)
        {
            var obj = new H10000Response
            {
                error = 1,
                data = new PagedListDto<H10000ResponseListItem>
                {
                    RecordList = new List<H10000ResponseListItem>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10000(new H10000Request
                {
                    page = dto.page,
                    pagesize = dto.pagesize
                });

                var response = responsebase as H10000Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null)
                {
                    obj = new H10000Response
                    {
                        error = 0,
                        data = new PagedListDto<H10000ResponseListItem>
                        {
                            Page = response.data.Page,
                            PageSize = response.data.PageSize,
                            RecordCount = response.data.RecordCount,
                            RecordList = response.data.RecordList

                        }
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return obj;
        }

        public static object Table(TableDto dto)
        {
            var obj = new H10000Response
            {
                error = 1,
                data = new PagedListDto<H10000ResponseListItem>
                {
                    RecordList = new List<H10000ResponseListItem>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10000(new H10000Request
                {
                    page = dto.page,
                    pagesize = dto.pagesize
                });

                var response = responsebase as H10000Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null)
                {
                    obj = new H10000Response
                    {
                        error = 0,
                        data = new PagedListDto<H10000ResponseListItem>
                        {
                            Page = response.data.Page,
                            PageSize = response.data.PageSize,
                            RecordCount = response.data.RecordCount,
                            RecordList = response.data.RecordList

                        }
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return obj;
        }
    }
}