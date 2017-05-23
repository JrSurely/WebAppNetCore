﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppNetCore
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class ConfiguredOptionsAttribute : Attribute
    {
        /// <summary>
        /// 创建 <see cref="ConfiguredOptionsAttribute"/> 的新实例。
        /// </summary>
        /// <param name="configurationSection">用于加载配置的配置节。</param>
        public ConfiguredOptionsAttribute(string configurationSection)
        {                                                                                              
            this.ConfigurationSection = configurationSection.Trim();
        }

        /// <summary>
        /// 表示用于加载配置的配置节。
        /// </summary>
        public string ConfigurationSection { get; }     
      
    }
  
}
