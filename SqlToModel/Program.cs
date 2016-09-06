using Agile.Data.Helpers;
using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SqlToModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入命名空间：");

            var input = Console.ReadLine();
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(new string[] { desktop, "Models" });
            if (!Directory.Exists(folder))
            {
                try
                {
                    Directory.CreateDirectory(folder);
                }
                catch
                {
                    Console.WriteLine("创建目录失败：" + folder);
                    Console.ReadKey();
                    return;
                }
            }

            var sw = new Stopwatch();
            sw.Start();

            var sqlstr = "select name as IName,1 as ICount from sysobjects where xtype='u'";
            var recordlist = new List<GroupItemDto>();
            try
            {
                recordlist = DataHelper.ExecuteList<GroupItemDto>(sqlstr);
            }
            catch
            { }

            if (recordlist == null || recordlist.Count == 0)
            {
                Console.WriteLine("获取数据库表失败");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("获取到{0}个表，开始生成类文件", recordlist.Count);
            foreach (var item in recordlist)
            {
                try
                {
                    CreateClassFile(item.IName, input, folder);
                }
                catch
                {
                    Console.WriteLine("生成{0}.cs失败", item.IName);
                    continue;
                }
            }

            sw.Stop();
            Console.WriteLine("操作已完成，耗时{0}秒", sw.Elapsed.TotalSeconds);
            Console.ReadKey();
        }

        static void CreateClassFile(string name, string namespacestr, string folder)
        {
            var sqlstr = string.Format("SELECT name,xtype,[length],collation,isnullable,colid from syscolumns where id=OBJECT_ID('{0}');", name);
            var recordlist = new List<tcolumn>();
            try
            {
                recordlist = DataHelper.ExecuteList<tcolumn>(sqlstr);
            }
            catch
            { }

            if (recordlist == null || recordlist.Count == 0)
            {
                Console.WriteLine("获取{0}表的字段失败", name);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendFormat("using System;\r\n");
            sb.AppendFormat("using System.ComponentModel.DataAnnotations;\r\n\r\n");
            sb.AppendFormat("namespace {0}\r\n", namespacestr);
            sb.AppendLine("{");
            sb.AppendFormat("    public class {0}\r\n", name);
            sb.AppendLine("    {");
            foreach (var item in recordlist)
            {
                if (item.colid == 1)
                {
                    sb.AppendFormat("        [Key]\r\n");
                }

                if (item.xtypestr == "string")
                {
                    sb.AppendFormat("        [MaxLength({0})]\r\n", item.lengthx);
                }

                if (item.isnullable == 0)
                {
                    sb.AppendFormat("        [Required]\r\n");
                }

                sb.AppendLine("        public " + item.xtypestr + " " + item.name + " {get;set;}\r\n");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            var str = sb.ToString();
            var filename = Path.Combine(new string[] { folder, name + ".cs" });
            using (var sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                sw.Write(str);
            }

            Console.WriteLine("已生成类文件：{0}.cs", name);
        }
    }

    public enum xtypeenum
    {
        ibyte = 48,

        iint = 56,

        idatetime = 61,

        ibool = 104,

        idecimal = 106,

        ilong = 127,

        invarchar = 231
    }

    public class tcolumn
    {
        public string name { get; set; }

        public int xtype { get; set; }

        public string xtypestr
        {
            get
            {
                var str = "string";
                switch (xtype)
                {
                    case (int)xtypeenum.idatetime:
                        {
                            str = "DateTime";
                        }
                        break;
                    case (int)xtypeenum.iint:
                        {
                            str = "int";
                        }
                        break;
                    case (int)xtypeenum.idecimal:
                        {
                            str = "decimal";
                        }
                        break;
                    case (int)xtypeenum.ilong:
                        {
                            str = "long";
                        }
                        break;
                    case (int)xtypeenum.ibool:
                        {
                            str = "bool";
                        }
                        break;
                    case (int)xtypeenum.ibyte:
                        {
                            str = "byte";
                        }
                        break;
                }

                return str;
            }
        }

        public int length { get; set; }

        public int lengthx
        {
            get
            {
                if (string.IsNullOrEmpty(collation))
                {
                    return length;
                }

                if (length == -1)
                {
                    return 4000;
                }

                if (length == 1)
                {
                    return 1;
                }

                return Convert.ToInt32(Math.Round(0.5 * length));
            }
        }

        public string collation { get; set; }

        public int isnullable { get; set; }

        public int colid { get; set; }
    }
}
