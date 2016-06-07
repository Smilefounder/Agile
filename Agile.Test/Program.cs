using Agile.Helpers;
using Agile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            QueryHelper.Delete<T_interface>(new T_interface
            {
                Id = 6
            });

            QueryHelper.Save<T_interface>(new T_interface
            {
                Code = "H1111",
                CreatedAt = DateTime.Now,
                Name = "测试"
            });

            QueryHelper.Update<T_interface>(new T_interface
            {
                Id = 8,
                Code = "H1aa1",
                CreatedAt = DateTime.Now,
                Name = "测试2x"
            });

            var list = QueryHelper.GetList<T_interface>(new QueryOptions { });
            foreach (var item in list)
            {
                Console.WriteLine("{0}:{1}", item.Code, item.Name);
            }

            var plist = QueryHelper.GetPagedList<T_interface>(new PagedQueryOptions
            {
                Page = 1,
                PageSize = 10
            });

            foreach (var item in plist.RecordList)
            {
                Console.WriteLine("{0}:{1}", item.Code, item.Name);
            }
        }
    }
}
