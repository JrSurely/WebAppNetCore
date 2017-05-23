using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;
using WebAppNetCore.Services;

namespace WebAppNetCore.Extensions
{
    public class RuntimeHelper
    {
        /// <summary>
        /// 获取项目程序集，排除所有的系统程序集(Microsoft.***、System.***等)、Nuget下载包
        /// </summary>
        /// <returns></returns>
        public static IList<Assembly> GetAllAssemblies()
        {
            var list = new List<Assembly>();
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");//排除所有的系统程序集、Nuget下载包   
            //  list= Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var lib in libs)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    list.Add(assembly);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return list;
        }

        public static Assembly GetAssembly(string assemblyName)
        {
            return GetAllAssemblies().FirstOrDefault(assembly => assembly.FullName.Contains(assemblyName));
        }

        public static IList<Type> GetAllTypes()
        {
            var list = new List<Type>();
            foreach (var assembly in GetAllAssemblies())
            {
                var typeInfos = assembly.DefinedTypes;
                foreach (var typeInfo in typeInfos)
                {
                    list.Add(typeInfo.AsType());
                }
            }
            return list;
        }

        public static IList<Type> GetTypesByAssembly(string assemblyName)
        {
            var list = new List<Type>();
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            var typeInfos = assembly.DefinedTypes;
            foreach (var typeInfo in typeInfos)
            {
                list.Add(typeInfo.AsType());
            }
            return list;
        }

        public static Type GetImplementType(string typeName, Type baseInterfaceType)
        {
            return GetAllTypes().FirstOrDefault(t =>
            {
                if (t.Name == typeName &&
                    t.GetTypeInfo().GetInterfaces().Any(b => b.Name == baseInterfaceType.Name))
                {
                    var typeInfo = t.GetTypeInfo();
                    return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType;
                }
                return false;
            });
        }

        public static void FindAssembly(string assemblyName)
        {
            Assembly asm = Assembly.Load(new AssemblyName(assemblyName));
            Type[] types = asm.GetExportedTypes().Where(t => t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsGenericType).ToArray();

            foreach (var type in types)
            {
                var interfaceTypes = type.GetTypeInfo().GetInterfaces().Where(itf => typeof(IDependency).GetTypeInfo().IsAssignableFrom(itf)).ToArray();
                if (interfaceTypes.Length == 0)
                {
                    continue;
                }
                bool useDependencyInterfaceOnly = IsDependencyInterfaceOnly(interfaceTypes);
                if (useDependencyInterfaceOnly)
                {
                    if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(type))
                    {

                    }
                    else if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(type))
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    //注册 IDependency，特别注意，如果一类型上直接使用了 IDependency 接口，理解为想要直接注入该类型。
                    foreach (var interfaceType in interfaceTypes.Where(i => !IsDependencyInterface(i)))
                    {
                        if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(interfaceType))
                        {

                        }
                        else if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(interfaceType))
                        {

                        }
                        else
                        {
                        }
                    }
                }
            }
        }
        private static bool IsDependencyInterface(Type interfaceType)
        {
            return interfaceType.Equals(typeof(IDependency)) || interfaceType.Equals(typeof(ISingletonDependency)) || interfaceType.Equals(typeof(ITransientDependency));
        }

        /// <summary>
        /// 是否只包含三种 <see cref="IDependency"/> 接口
        /// </summary>
        private static bool IsDependencyInterfaceOnly(IEnumerable<Type> interfaceTypes)
        {
            int interfaceCount = interfaceTypes.Count();

            return interfaceCount > 0 && (!interfaceTypes.Any(i => !IsDependencyInterface(i))); //所有接口类型中不包含不是 Dependency 接口的类型。
        }

    }
}
