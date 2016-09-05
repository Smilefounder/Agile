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

            Query("reimu", 1, 1);
        }

        private static void Query(string username, int? status, int? orderby)
        {
            var options = new QueryOptions();
            if (orderby.HasValue)
            {
                switch (orderby.Value)
                {
                    case 1:
                        {
                            options.OrderByDesc<T_user>(w => new { w.UserName, w.CreatedAt });
                        }
                        break;
                    default:
                        {
                            options.OrderByDesc<T_user>(w => w.Id);
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(username))
            {
                options.Where<T_user>(w => w.UserName == username);
            }

            if (status.HasValue)
            {
                options.Where<T_user>(w => w.Status == status.Value);
            }

            var list = new List<T_user>();
            try
            {
                list = QueryHelper.GetList<T_user>(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            list.ForEach(f =>
            {
                Console.WriteLine("{0} {1} {2}", f.Id, f.UserName, f.Status);
            });
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
            }
        }
    }
}
