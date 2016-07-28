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
    }
}
