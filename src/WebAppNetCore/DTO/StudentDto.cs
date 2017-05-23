using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Attributes;
using WebAppNetCore.Validators;

namespace WebAppNetCore.DTO
{
    [Validator(typeof(StudentDtoValidator))]
    public class StudentDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
