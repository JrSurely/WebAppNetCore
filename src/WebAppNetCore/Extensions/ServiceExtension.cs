using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAppNetCore.Extensions;
using WebAppNetCore.Services;

namespace Microsoft.Extensions.DependencyInjection // WebAppNetCore.Extensions
{
    /// <summary>
    /// IServiceCollection扩展
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 用DI批量注入接口程序集中对应的实现类。
        /// <para>
        /// 需要注意的是，这里有如下约定：
        /// IUserService --> UserService, IUserRepository --> UserRepository.
        /// </para>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="interfaceAssemblyName">接口程序集的名称（不包含文件扩展名）</param>
        /// <returns></returns>
        public static IServiceCollection RegisterAssembly(this IServiceCollection service, string interfaceAssemblyName)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrEmpty(interfaceAssemblyName))
                throw new ArgumentNullException(nameof(interfaceAssemblyName));

            var assembly = RuntimeHelper.GetAssembly(interfaceAssemblyName);
            if (assembly == null)
            {
                throw new DllNotFoundException($"the dll \"{interfaceAssemblyName}\" not be found");
            }

            //过滤掉非接口及泛型接口
            var types = assembly.GetTypes().Where(t => t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsGenericType);

            foreach (var type in types)
            {
                var implementTypeName = type.Name.Substring(1);
                var implementType = RuntimeHelper.GetImplementType(implementTypeName, type);
                if (implementType != null)
                    service.AddSingleton(type, implementType);
            }
            return service;
        }
        public static IServiceCollection FindAssembly(this IServiceCollection service, string assemblyName)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));
            Assembly asm = Assembly.Load(new AssemblyName(assemblyName));
            //过滤掉非接口及泛型接口
            Type[] types = asm.GetExportedTypes().Where(t => t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsGenericType).ToArray();

            foreach (var type in types)
            {
                var interfaceTypes = type.GetTypeInfo().GetInterfaces().Where(itf => typeof(IDependency).GetTypeInfo().IsAssignableFrom(itf)).ToArray();
                if (interfaceTypes.Length == 0)
                {
                    continue;
                }
                bool useDependencyInterfaceOnly = IsDependencyInterfaceOnly(interfaceTypes);


                //var implementTypeName = type.Name.Substring(1);
                //var implementType = RuntimeHelper.GetImplementType(implementTypeName, type);
                Type[] implementTypes = asm.GetExportedTypes().Where(t => t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsGenericType).ToArray();
                if (useDependencyInterfaceOnly)
                {
                    foreach (var implementType in implementTypes)
                    {
                        if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(type))
                        {
                            service.AddSingleton(type, implementType);
                        }
                        else if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(type))
                        {
                            service.AddTransient(type, implementType);
                        }
                        else if (typeof(IDependency).GetTypeInfo().IsAssignableFrom(type))
                        {
                            service.AddScoped(type, implementType);
                        }
                    }

                }
                else
                {
                    //注册 IDependency，特别注意，如果一类型上直接使用了 IDependency 接口，理解为想要直接注入该类型。
                    foreach (var interfaceType in interfaceTypes.Where(i => !IsDependencyInterface(i)))
                    {

                        foreach (var implementType in implementTypes)
                        {
                            if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(interfaceType))
                            {
                                service.AddSingleton(type, implementType);
                            }
                            else if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(interfaceType))
                            {
                                service.AddTransient(type, implementType);
                            }
                            else if (typeof(IDependency).GetTypeInfo().IsAssignableFrom(type))
                            {
                                service.AddScoped(type, implementType);
                            }
                        }
                    }
                }
            }
            return service;
        }
        public static bool MyInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }
        private static bool IsDependencyInterface(Type interfaceType)
        {
            return interfaceType.Equals(typeof(IDependency)) || interfaceType.Equals(typeof(ISingletonDependency)) || interfaceType.Equals(typeof(ITransientDependency));
        }

        /// <summary>
        /// 是否只包含三种<see cref="IDependency"/> 接口
        /// </summary>
        private static bool IsDependencyInterfaceOnly(IEnumerable<Type> interfaceTypes)
        {
            int interfaceCount = interfaceTypes.Count();

            return interfaceCount > 0 && (!interfaceTypes.Any(i => !IsDependencyInterface(i))); //所有接口类型中不包含不是 Dependency 接口的类型。
        }

        /// <summary>
        /// 用DI批量注入接口程序集中对应的实现类。
        /// </summary>
        /// <param name="service"></param>
        /// <param name="interfaceAssemblyName">接口程序集的名称（不包含文件扩展名）</param>
        /// <param name="implementAssemblyName">实现程序集的名称（不包含文件扩展名）</param>
        /// <returns></returns>
        public static IServiceCollection RegisterAssembly(this IServiceCollection service, string interfaceAssemblyName, string implementAssemblyName)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrEmpty(interfaceAssemblyName))
                throw new ArgumentNullException(nameof(interfaceAssemblyName));
            if (string.IsNullOrEmpty(implementAssemblyName))
                throw new ArgumentNullException(nameof(implementAssemblyName));

            var interfaceAssembly = RuntimeHelper.GetAssembly(interfaceAssemblyName);
            if (interfaceAssembly == null)
            {
                throw new DllNotFoundException($"the dll \"{interfaceAssemblyName}\" not be found");
            }

            var implementAssembly = RuntimeHelper.GetAssembly(implementAssemblyName);
            if (implementAssembly == null)
            {
                throw new DllNotFoundException($"the dll \"{implementAssemblyName}\" not be found");
            }

            //过滤掉非接口及泛型接口
            var types = interfaceAssembly.GetTypes().Where(t => t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsGenericType);

            foreach (var type in types)
            {
                //过滤掉抽象类、泛型类以及非class
                var implementType = implementAssembly.DefinedTypes
                    .FirstOrDefault(t => t.IsClass && !t.IsAbstract && !t.IsGenericType &&
                                         t.GetInterfaces().Any(b => b.Name == type.Name));
                if (implementType != null)
                {
                    service.AddSingleton(type, implementType.AsType());
                }
            }

            return service;
        }
    }
}
