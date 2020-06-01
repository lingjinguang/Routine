using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routine.Api.Entities;
using Routine.Api.Models;

namespace Routine.Api.Profiles
{
    public class EmployeeProfile:Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee,EmployeeDto>()
                .ForMember(destinationMember:dest=>dest.Name
                    , memberOptions:opt=>opt.MapFrom(mapExpression:src=>$"{src.FirstName} {src.LastName}"))
                .ForMember(destinationMember:dest=>dest.GenderDisplay
                    ,memberOptions:opt=>opt.MapFrom(mapExpression:src=>src.Gender.ToString()))
                .ForMember(destinationMember:dest=>dest.age
                    ,memberOptions:opt=>opt.MapFrom(mapExpression:src=>DateTime.Now.Year-src.DateOfBirth.Year));
            CreateMap<EmployeeAddDto, Employee>();
            CreateMap<EmployeeUpdateDto,Employee>();
            CreateMap<Employee,EmployeeUpdateDto>();
        }
    }
}
