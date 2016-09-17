using Agile.Attributes;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ModelToSql
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("请输入Models程序集文件路径，按Enter开始生成Sql，按Q退出：");

                var input = Console.ReadLine();
                if (input.ToUpper() == "Q")
                {
                    break;
                }

                if (!File.Exists(input))
                {
                    Console.WriteLine("文件路径不正确");
                    Console.WriteLine();
                    continue;
                }

                var fullname = default(string);
                BuildSql(input, ref fullname);
                Console.WriteLine("文件已生成到桌面：" + fullname);
                Console.WriteLine();
                continue;
            }
        }

        static void BuildSql(string filepath, ref string fullname)
        {
            var sb = new StringBuilder();
            var assembly = Assembly.LoadFrom(filepath);
            var types = assembly.GetTypes();
            foreach (var t in types)
            {
                if (!t.IsClass)
                {
                    continue;
                }

                if (t.Name.ToUpper() == "T_BASE")
                {
                    continue;
                }

                sb.AppendFormat("CREATE TABLE {0}(\r\n", t.Name);
                var tps = t.GetProperties();
                var sb2 = new StringBuilder();
                for (var i = 0; i < tps.Length; i++)
                {
                    var tp = tps[i];
                    var field = ParseField(tp);
                    if (string.IsNullOrEmpty(field))
                    {
                        continue;
                    }

                    if (i < tps.Length - 1)
                    {
                        sb2.AppendFormat("{0},\r\n", field);
                    }
                    else
                    {
                        sb2.AppendFormat("{0}\r\n", field);
                    }
                }

                var sb2str = sb2.ToString();
                sb.AppendFormat(sb2str);
                sb.AppendFormat(");\r\n\r\n");
                sb.AppendFormat("GO\r\n\r\n");
            }

            var dt = DateTime.Now;
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var filename = String.Format("tables_{0}.txt", dt.ToString("yyyy_MM_dd"));

            fullname = Path.Combine(new string[] { desktop, filename });
            using (var sw = new StreamWriter(fullname, false, Encoding.UTF8))
            {
                sw.Write(sb.ToString());
            }
        }

        static string ParseField(PropertyInfo p)
        {
            var field = p.GetCustomAttribute<TableFieldAttribute>();
            if (field == null)
            {
                return null;
            }

            var fieldstr = "";
            fieldstr += string.Format("    [{0}] ", p.Name);

            var ptstr = p.PropertyType.ToString();
            if (ptstr.Contains(typeof(int).FullName))
            {
                fieldstr += "INT ";
            }

            else if (ptstr.Contains(typeof(decimal).FullName))
            {
                fieldstr += string.Format("DECIMAL({0},2) ", field.MaxLength > 0 ? field.MaxLength : 18);
            }

            else if (ptstr.Contains(typeof(DateTime).FullName))
            {
                fieldstr += "DATETIME ";
            }

            else
            {
                fieldstr += string.Format("NVARCHAR({0}) ", field.MaxLength);
            }

            if (field.IsIdentity)
            {
                fieldstr += "IDENTITY(1,1) ";
            }

            if (field.IsPrimaryKey)
            {
                fieldstr += "PRIMARY KEY ";
            }

            if (field.IsNotNull)
            {
                fieldstr += "NOT NULL ";
            }
            else
            {
                fieldstr += "NULL ";
            }

            return fieldstr;
        }
    }
}
