using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppNetCore.Services
{
    public interface IStudentService: IDependency
    {
        string QueryStudentCode();
    }
}
