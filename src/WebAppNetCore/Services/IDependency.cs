using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppNetCore.Services
{
    /// <summary>
    /// scope生命周期
    /// </summary>
    public interface IDependency
    {
    }
    /// <summary>
    /// Singleton生命周期
    /// </summary>
    public interface ISingletonDependency
    {
    }
    /// <summary>
    /// Transisent生命周期
    /// </summary>
    public interface ITransientDependency      
    {
    }
}
