using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppNetCore.Services
{
    public class StudentService : IStudentService
    {
        private Guid _id;

        public StudentService()
        {
            _id = Guid.NewGuid();
        }
        public string QueryStudentCode()
        {
            return $"学生编号为：{_id}";
        }
    }
}
