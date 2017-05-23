using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using WebAppNetCore.DTO;

namespace WebAppNetCore.Validators
{
    public class StudentDtoValidator:AbstractValidator<StudentDto>
    {
        public StudentDtoValidator()
        {
            RuleFor(x=>x.Code).NotEmpty().WithMessage($"{nameof(StudentDto.Code)}不能为空").NotNull().WithMessage($"{nameof(StudentDto.Code)}不能为空");
        }
    }
}
