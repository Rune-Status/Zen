using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Zen.Util
{
    public class AssemblyUtil
    {
        public static Dictionary<object, MethodInfo> GetMethodsWithAttribute<T>(Assembly parentAssembly = null)
            where T : Attribute
        {
            var assembly = parentAssembly ?? Assembly.GetCallingAssembly();

            return assembly.GetTypes().SelectMany(x => x.GetMethods())
                .Where(y => y.GetCustomAttributes(typeof(T), true).Length > 0)
                .ToDictionary(z =>
                {
                    Debug.Assert(z.DeclaringType != null, "z.DeclaringType != null");
                    return Activator.CreateInstance(z.DeclaringType);
                });
        }
    }
}