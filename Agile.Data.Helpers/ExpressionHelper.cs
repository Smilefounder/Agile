using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
                    var unaryExp = lambdaExp.Body as UnaryExpression;
                    if (unaryExp != null)
                    {
                        var memberExp2 = unaryExp.Operand as MemberExpression;
                        if (memberExp2 != null)
                        {
                            names.Add(memberExp2.Member.Name);
                        }
                    }

                    var newExp = lambdaExp.Body as NewExpression;
                    if (newExp != null)
                    {
                        foreach (var member in newExp.Members)
                        {
                            names.Add(member.Name);
                        }
                    }

                    var memberExp = lambdaExp.Body as MemberExpression;
                    if (memberExp != null)
                    {
                        if (memberExp.Type == typeof(string) || memberExp.Type.IsValueType)
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
                sb += " AND (";
                ExpressionRouter(binaryExp.Left, ref sb);

                var expType = ExpressionTypeConverter(binaryExp.NodeType);
                if (!String.IsNullOrEmpty(expType))
                {
                    sb += expType;
                }

                ExpressionRouter(binaryExp.Right, ref sb);
                sb += ") ";
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
                ConstantExpressionValueConverter(constantExp, ref sb);
                return;
            }
        }

        private static void ConstantExpressionValueConverter(ConstantExpression constantExp, ref string sb)
        {
            var typeName = constantExp.Type.Name;
            if (typeName == typeof(int).Name || typeName == typeof(long).Name || typeName == typeof(decimal).Name)
            {
                sb += constantExp.Value;
                return;
            }

            if (typeName == typeof(DateTime).Name)
            {
                var dt = (DateTime)constantExp.Value;
                sb += "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            sb += "'" + constantExp.Value.ToString() + "'";
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
