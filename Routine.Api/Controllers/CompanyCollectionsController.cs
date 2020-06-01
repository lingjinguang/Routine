using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route(template:"api/CompanyCollections")]
    public class CompanyCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        public CompanyCollectionsController(IMapper mapper,ICompanyRepository companyRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }
        [HttpGet(template: "({ids})", Name = nameof(GetCompanyCollection))]
        public async Task<IActionResult> GetCompanyCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] 
            IEnumerable<Guid> ids)
         {
            if (ids == null)
            {
                return BadRequest();
            }
            var entities = await _companyRepository.GetCompaniesAsync(ids);
            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }
            var dotsToResult = _mapper.Map<IEnumerable<CompanyDto>>(entities);
            return Ok(dotsToResult);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> CreateCompanyCollection(IEnumerable<CompanyAddDto> companyCollection)
        {
            var companyEntity = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntity)
            {
                 _companyRepository.AddCompany(company);
            }

            await _companyRepository.SaveAsync();
            var dotToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntity);
            var idsString = string.Join(separator:",",values:dotToReturn.Select(x=>x.Id));
            return CreatedAtRoute(nameof(GetCompanyCollection),
                routeValues:new { ids = idsString },
                value: dotToReturn);
        }
    }
}