using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Attributes
{
    /// <summary>
    /// 贴上该标签表示登录后即可访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LoginAccessAttribute : Attribute
    {
    }
}
