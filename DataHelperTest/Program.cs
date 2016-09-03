using Agile.Data.Helpers;
using Agile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHelperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Save();

            //Update();

            var status = 1;
            var options = new QueryOptions();
            options.OrderByDesc<T_user>(w => new { w.Id });
            options.Where<T_user>(w => w.Id == 1);
            options.Where<T_user>(w => w.Status == status);

            var list = QueryHelper.GetList<T_user>(options);
        }

        static void Save()
        {
            try
            {
                var model = new T_user
                {
                    CreatedAt = DateTime.Now,
                    Domain = 1,
                    Email = "i@uimoe.com",
                    Status = 1,
                    UserName = "reimu",
                    UserPass = "reimu1118"
                };

                var rows = WriteHelper.Save<T_user>(model);
                Console.WriteLine(rows);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Update()
        {
            try
            {
                var model = new T_user
                {
                    CreatedAt = DateTime.Now,
                    Domain = null,
                    Email = "i@llyn23.com",
                    Id = 1,
                    Status = 0,
                    UserName = "xxx",
                    UserPass = "yyy"
                };

                var rows = WriteHelper.Update<T_user>(model);
                Console.WriteLine(rows);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
