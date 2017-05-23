using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebAppNetCore;
using WebAppNetCore.DTO;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreExtension
    {
        public static void AddCofiguration(this IServiceCollection services, IConfiguration configuration)
        {                
            //System.Reflection.Assembly asm =
            //System.Reflection.Assembly.Load("WebAppNetCore");     
            Assembly asm = Assembly.Load(new AssemblyName("WebAppNetCore"));
            Type[] types = asm.GetExportedTypes();

            #region  查找具有 ConfiguredOptions 特性的类  
            // 验证指定自定义属性  方法1
            //Func<Attribute[], bool> isConfigureted = o =>
            //{
            //    foreach (Attribute attr in o)
            //    {
            //        if (attr is ConfiguredOptionsAttribute)
            //            return true;
            //    }
            //    return false;
            //};
            //Type[] cosType = types.Where(o =>isConfigureted(o.GetTypeInfo().GetCustomAttributes(typeof(ConfiguredOptionsAttribute), true).ToArray())).ToArray();

            //方法2
          //  Type[] cosType= types.Where(x => x.GetTypeInfo().GetCustomAttributes(typeof(ConfiguredOptionsAttribute), true).Any()).ToArray();
            //方法3
            Type[] cosType = types.Where(x => x.GetTypeInfo().HasAttribute<ConfiguredOptionsAttribute>()).ToArray();
            #endregion        
            Lazy<Type> configurationOptionsType = new Lazy<Type>(() => typeof(ConfigureFromConfigurationOptions<>));
            Lazy<Type> configurationInterfaceType = new Lazy<Type>(() => typeof(IConfigureOptions<>));
            //遍历具有指定特性的类型     
            foreach (Type options in cosType)
            {
                Type interfaceType = configurationInterfaceType.Value.MakeGenericType(options);
                Type opType = configurationOptionsType.Value.MakeGenericType(options);
                IConfiguration config = configuration.GetSection(options.Name.ToString());//约定节点名称和类名一样
                object instance = System.Activator.CreateInstance(opType, args: config);
                services.AddSingleton(interfaceType, instance);
            }
        }
        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherit);                                       
        }
    }
}
