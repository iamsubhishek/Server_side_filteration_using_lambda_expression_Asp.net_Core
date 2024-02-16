using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DataTableServerSide_GenericSortNdSearchMethod.Helper
{
    public class SearchHelper
    {
        public static Expression<Func<T, bool>> BuildLambda<T>(string propertyName, string searchval)
        {
            var Type = Expression.Parameter(typeof(T));
            var prop = Expression.Property(Type, propertyName);
            var searchconstant = Expression.Constant(searchval);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var expressionCall = Expression.Call(prop, method, searchconstant);

            var lambda = Expression.Lambda<Func<T, bool>>(expressionCall, Type);
            return lambda;
        }


        public static IQueryable<T> SearchInAllColumns<T>(IEnumerable<T> obj, string searchkey)
        {
            var coll = obj.ToList();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (properties == null)
                throw new ArgumentException("{typeof(T).Name}' does not implement a public get property named '{key}.");
            var filteredObj = obj.Where(d => properties.Any(p => p.GetValue(d) != null && p.GetValue(d).ToString().Contains(searchkey))).AsQueryable();
            return filteredObj;
        }
    }
}
