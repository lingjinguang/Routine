using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routine.Api.Entities;
using Routine.Api.Models;

namespace Routine.Api.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            //当名称不一致时，可使用ForMember手动映射
            CreateMap<Company, CompanyDto>()
                .ForMember(
                destinationMember:dest=>dest.CompanyName,
                memberOptions:opt=>opt.MapFrom(mapExpression:src=>src.Name));
            CreateMap<CompanyAddDto, Company>();
            CreateMap<Company, CompanyFullDto>();
        }
    }
}
