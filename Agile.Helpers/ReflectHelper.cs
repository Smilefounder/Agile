using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Agile.Helpers
{
    public class ReflectHelper
    {
        public static T Copy<T, K>(K source)
            where T : class
            where K : class
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();

            var ktype = typeof(K);
            var kps = ktype.GetProperties();

            var target = (T)Activator.CreateInstance(ttype);
            foreach (var tp in tps)
            {
                var kp = kps.Where(w => w.Name == tp.Name).FirstOrDefault();
                if (kp == null)
                {
                    continue;
                }

                var obj = kp.GetValue(source);
                try
                {
                    tp.SetValue(target, obj);
                }
                catch
                {
                    continue;
                }
            }

            return target;
        }

        public static void CopyTo<T, K>(T target, K source)
            where T : class
            where K : class
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();

            var ktype = typeof(K);
            var kps = ktype.GetProperties();

            foreach (var tp in tps)
            {
                var kp = kps.Where(w => w.Name == tp.Name).FirstOrDefault();
                if (kp == null)
                {
                    continue;
                }

                var obj = kp.GetValue(source);
                if (obj == null)
                {
                    continue;
                }

                try
                {
                    tp.SetValue(target, obj);
                }
                catch
                {
                    continue;
                }
            }
        }

        public static List<T> DataTableToList<T>(DataTable table)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var item = (T)Activator.CreateInstance(ttype);
                foreach (var p in tps)
                {
                    try
                    {
                        p.SetValue(item, row[p.Name], null);
                    }
                    catch
                    {
                        continue;
                    }
                }
                list.Add(item);
            }

            return list;
        }
    }
}
