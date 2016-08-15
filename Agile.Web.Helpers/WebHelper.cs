using System;
using System.Linq;
using System.Text;
using System.Web;

namespace Agile.Web.Helpers
{
    public class WebHelper
    {
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

        public static string UrlEncode(string input)
        {
            return HttpUtility.UrlEncode(input, Encoding.GetEncoding("GBK"));
        }

        public static string ToRequestStr<T>(T entity)
            where T : class
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();

            var sb = "";
            foreach (var tp in tps)
            {
                object obj = null;
                try
                {
                    if (entity != null)
                    {
                        obj = tp.GetValue(entity, null);
                    }
                }
                catch
                { }

                sb += string.Format("&{0}={1}", tp.Name, obj == null ? "" : obj.ToString());
            }

            if (sb.Length > 0)
            {
                sb = sb.Substring(1);
            }

            return sb;
        }
    }
}
