using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Metaseed
{
    public class ExpressionHelper
    {
        /// <summary>
        /// Catel.ExpressionHelper.GetPropertyName work, but
        /// Catel.ExpressionHelper.GetOwner do not work, so we build our own
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
     public static  Tuple<object, string> GetPropertyOwnerAndName<Tproperty>(Expression<Func<Tproperty>> property)
        {
            var mex = property.Body as MemberExpression;
            string name = mex.Member.Name;
            var fex = mex.Expression as MemberExpression;
            var cex = fex.Expression as ConstantExpression;
            var fld = fex.Member as System.Reflection.FieldInfo;
            var obj = fld.GetValue(cex.Value);
            return new Tuple<object, string>(obj, name);
        }
    }
}
