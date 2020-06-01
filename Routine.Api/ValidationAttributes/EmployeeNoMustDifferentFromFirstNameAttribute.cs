using Routine.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.ValidationAttributes
{
    public class EmployeeNoMustDifferentFromFirstNameAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //value 只会返回属性值  validationContext.ObjectInstance;总是能获取到值
            var addDto = (EmployeeAddDto) validationContext.ObjectInstance;
            if(addDto.EmployeeNo == addDto.FirstName)
            {
                return new ValidationResult(ErrorMessage,new [] { nameof(addDto.EmployeeNo) }); 
            }
            return ValidationResult.Success;
        }
    }
}
