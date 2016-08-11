using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Data.Helpers
{
    public class ExpressionHelper
    {
        public static string[] GetFields(params Expression[] explist)
        {
            var names = new List<string>();

            foreach (var exp in explist)
            {
                var lambdaExp = exp as LambdaExpression;
                if (lambdaExp != null)
                {
                    var memberExp = lambdaExp.Body as MemberExpression;
                    if (memberExp != null)
                    {
                        var ttype = memberExp.Type;
                        if (ttype == typeof(string) || ttype.IsValueType)
                        {
                            names.Add(memberExp.Member.Name);
                        }
                    }
                }
            }

            return names.ToArray();
        }

        public static string MakeWhereStr(params Expression[] explist)
        {
            var sb = "";
            foreach (var exp in explist)
            {
                var lambdaExp = exp as LambdaExpression;
                if (lambdaExp != null)
                {
                    ExpressionRouter(lambdaExp.Body, ref sb);
                }
            }

            return sb;
        }

        private static void ExpressionRouter(Expression exp, ref string sb)
        {
            var binaryExp = exp as BinaryExpression;
            if (binaryExp != null)
            {
                ExpressionRouter(binaryExp.Left, ref sb);

                var expType = ExpressionTypeConverter(binaryExp.NodeType);
                if (!String.IsNullOrEmpty(expType))
                {
                    sb += expType;
                }

                ExpressionRouter(binaryExp.Right, ref sb);
                return;
            }

            var memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                sb += memberExp.Member.Name;
                return;
            }

            var constantExp = exp as ConstantExpression;
            if (constantExp != null)
            {
                sb += "'" + constantExp.Value.ToString() + "'";
                return;
            }
        }

        private static string ExpressionTypeConverter(ExpressionType expType)
        {
            switch (expType)
            {
                default:
                    return null;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " =";
                case ExpressionType.GreaterThan:
                    return " >";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " Or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
            }
        }
    }
}
