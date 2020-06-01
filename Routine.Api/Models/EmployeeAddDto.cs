using Routine.Api.Entities;
using Routine.Api.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Routine.Api.Models
{
    [EmployeeNoMustDifferentFromFirstNameAttribute(ErrorMessage = "员工号不能和名一样！！")]
    public class EmployeeAddDto/*:IValidatableObject*/
    {
        [Display(Name ="员工号")]
        [Required(ErrorMessage = "{0}是必填的")]
        [StringLength(10,MinimumLength = 10,ErrorMessage ="{0}的长度是{1}")]
        public string EmployeeNo { get; set; }
        [Display(Name = "名")]
        [Required(ErrorMessage = "{0}是必填的")]
        [StringLength(50,ErrorMessage = "{0}的长度是{1}")]
        public string FirstName { get; set; }
        [Display(Name = "姓"),Required(ErrorMessage = "{0}是必填的"),MaxLength(50,ErrorMessage ="{0}的长度是{1}")]
        
        public string LastName { get; set; }
        [Display(Name = "性别")]
        public Gender Gender { get; set; }
        [Display(Name = "出生日期")]
        public DateTime DateOfBirth { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (FirstName == LastName)
        //    {
        //        yield return new ValidationResult("姓和名不能一样", new[] { nameof(FirstName), nameof(LastName) });
        //    }
        //}
    }
}
