using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Zen.Util
{
    public class AssemblyUtil
    {
        public static IEnumerable<Type> GetTypesWithInterface<T>() where T : class
        {
            var typeOf = typeof(T);
            var assembly = Assembly.GetCallingAssembly();

            return assembly.GetTypes().Where(x => !x.IsInterface && typeOf.IsAssignableFrom(x)).ToList();
        }
    }
}