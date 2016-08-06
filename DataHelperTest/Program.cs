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

            Update();
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

                var rows = QueryHelper.Save<T_user>(model);
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

                var rows = QueryHelper.Update<T_user>(model);
                Console.WriteLine(rows);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
