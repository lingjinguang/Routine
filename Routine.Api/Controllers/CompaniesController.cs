using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
//using System.Web.Http.Cors;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Route(template:"api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService; 

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper,IPropertyMappingService propertyMappingService,IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        [EnableCors("myAllowSpecificOrigins2")] // 测试
        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead]
        //public async Task<IActionResult> GetCompanies()
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyDtoParameters parameters, [FromHeader(Name = "Accept")] string mediaType)  //推荐使用这种能方式，便于生成文档 方案2
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            if (!_propertyMappingService.ValidMappingExistsFor<CompanyDto, Company>(parameters.OrderBy))
            {
                return BadRequest();
            }
            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }
            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            //var previousPageLink = companies.HasPrevious ? CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage):null;
            //var nextPageLink = companies.HasNext ? CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage) : null;
            var paginationMetadata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages
                //previousPageLink,
                //nextPageLink
            };

            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(paginationMetadata,new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            var shapedData = companyDtos.ShapeData(parameters.Fields);

            //var inCludeLinks = parsedMediaType.SubTypeWithoutSuffix.Substring()  Vendor-specific Media Types输出 

            if (parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
            {
                var links = CreateLinksForCompany(parameters, companies.HasPrevious, companies.HasNext);
                //{value:{xxx},links}
                var shapedCompaniesWithLinks = shapedData.Select(c =>
                {
                    var companyDict = c as IDictionary<string, object>;
                    var companyLinks = CreateLinksForCompany((Guid)companyDict["Id"], null);
                    companyDict.Add("links", companyLinks);
                    return companyDict;
                });
                var linkedCollectionResoure = new
                {
                    value = shapedCompaniesWithLinks,
                    links
                };
                return Ok(linkedCollectionResoure);//状态码200
            }

            return Ok(shapedData);//状态码200
            //return companyDtos;//方案2 试用该写法  return Ok();
        }
        [Produces("application/json",
           "application/vnd.company.hateoas+json",
           "application/vnd.company.company.friendly+json",
           "application/vnd.company.company.friendly.hateoas+json",
           "application/vnd.company.company.full+json",
           "application/vnd.company.company.full.hateoas+json")]
        [HttpGet(template:"{companyId}",Name = nameof(GetCompany))]
        //[Route("companyId")]
        public async Task<IActionResult> GetCompany(Guid companyId,string fields,[FromHeader(Name ="Accept")] string mediaType)
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType,out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }
            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> myLinks = new List<LinkDto>();
            if (includeLinks)
            {
                myLinks = CreateLinksForCompany(companyId,fields);
            }
            var primaryMediaType = includeLinks 
                ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) 
                : parsedMediaType.SubTypeWithoutSuffix;
            if (primaryMediaType == "vnd.company.company.full")
            {
                var full = _mapper.Map<CompanyFullDto>(company).ShapeData(fields) as IDictionary<string,object>;
                if (includeLinks)
                {
                    full.Add("links",myLinks);
                }
                return Ok(full);
            }
            var friendly = _mapper.Map<CompanyDto>(company).ShapeData(fields) as IDictionary<string,object>;
            if (includeLinks)
            {
                friendly.Add("links",myLinks);
            }
            return Ok(friendly);
            //if(parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
            //{
            //    var links = CreateLinksForCompany(companyId, fields);
            //    var linkDict = _mapper.Map<CompanyDto>(company).ShapeData(fields) as IDictionary<string, object>;
            //    linkDict.Add("links", links);
            //    return Ok(linkDict);//状态码200
            //}

            //return Ok(_mapper.Map<CompanyDto>(company).ShapeData(fields));//状态码200
        }
        [HttpPost(Name = nameof(CreateCompany))]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var resutDto = _mapper.Map<CompanyDto>(entity);
            var links = CreateLinksForCompany(resutDto.Id,null);
            var linkedDict = resutDto.ShapeData(null) as IDictionary<string,object>;
            linkedDict.Add("links", links);

            return CreatedAtRoute(nameof(GetCompany), routeValues: new { companyId = resutDto.Id }, linkedDict);
        }
        [HttpDelete("{companyId}",Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);
            if (companyEntity == null)
            {
                return NotFound();
            }
            await _companyRepository.GetEmployeesAsync(companyId,null);
            _companyRepository.DeleteCompany(companyEntity);

            await _companyRepository.SaveAsync();

            return NoContent();
        }
        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters,ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanies),new 
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }
        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId }),
                    "self",
                    "get"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId, fields }),
                    "self",
                    "get"));
            }
            links.Add(new LinkDto(Url.Link(nameof(DeleteCompany), new { companyId }),
                "delete_company",
                "DELETE"));
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.CreateEmployeeForCompany), new { companyId }),
                "create_employee_for_company",
                "POST"));
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.GetEmployeesByCompany), new { companyId }),
                "employees",
                "GET"));

            return links;
        }
        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters,bool hasPrevious,bool hasNext)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters,ResourceUriType.CurrentPage),"self","GET"));
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage), "previous_page", "GET"));
            }
            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage), "next_page", "GET"));
            }
            return links;
        }
    }
}