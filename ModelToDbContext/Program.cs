using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModelToDbContext
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
                BuildRepositoryFile(input, ref fullname);
                Console.WriteLine("文件已生成到桌面：" + fullname);
                Console.WriteLine();
                continue;
            }
        }

        private static void BuildRepositoryFile(string input, ref string fullname)
        {

            var sb = new StringBuilder();
            var assembly = Assembly.LoadFrom(input);
            var types = assembly.GetTypes();

            sb.AppendFormat("using System.Data.Entity;\r\n\r\n");
            sb.AppendFormat("namespace PF.Web.Helpers\r\n");
            sb.AppendLine("{");
            sb.AppendLine("    public class RepositoryHelper : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine("        public RepositoryHelper() : base(\"DefaultConnection\")");
            sb.AppendLine("        {\r\n");
            sb.AppendLine("        }\r\n");

            foreach (var t in types)
            {
                if (!t.IsClass)
                {
                    continue;
                }

                sb.AppendLine("        public DbSet<" + t.Name + "> " + t.Name + " { get; set; }\r\n");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var filename = "RepositoryHelper.cs";

            fullname = Path.Combine(new string[] { desktop, filename });
            using (var sw = new StreamWriter(fullname, false, Encoding.UTF8))
            {
                sw.Write(sb.ToString());
            }
        }
    }
}
