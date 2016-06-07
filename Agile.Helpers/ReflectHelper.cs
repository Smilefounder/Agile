using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        public static T ParseFromRequest<T>()
            where T : class
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();

            var entity = (T)Activator.CreateInstance(ttype);
            var context = HttpContext.Current;
            if (context == null)
            {
                return entity;
            }

            foreach (var tp in tps)
            {
                var key = context.Request.Params.AllKeys.Where(w => w.ToLower() == tp.Name.ToLower()).FirstOrDefault();
                if (String.IsNullOrEmpty(key))
                {
                    continue;
                }

                var objstr = context.Request.Params[key];

                try
                {
                    if (tp.PropertyType == typeof(string))
                    {
                        tp.SetValue(entity, objstr, null);
                        continue;
                    }

                    if (tp.PropertyType == typeof(int?))
                    {
                        tp.SetValue(entity, Convert.ToInt32(objstr), null);
                        continue;
                    }

                    if (tp.PropertyType == typeof(DateTime?))
                    {
                        tp.SetValue(entity, Convert.ToDateTime(objstr), null);
                        continue;
                    }

                    if (tp.PropertyType == typeof(bool?))
                    {
                        tp.SetValue(entity, Convert.ToBoolean(objstr), null);
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return entity;
        }
    }
}
