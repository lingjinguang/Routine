using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    //[ResponseCache(CacheProfileName = "120sCacheProfile")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class EmployeesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IPropertyCheckerService _propertyCheckerServeice;
        public EmployeesController(IMapper mapper, ICompanyRepository companyRepository,IPropertyCheckerService propertyCheckerServeice)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _propertyCheckerServeice = propertyCheckerServeice ?? throw new ArgumentNullException(nameof(propertyCheckerServeice));
        }

        [EnableCors("myAllowSpecificOrigins2")] // 测试
        [HttpGet(Name = nameof(GetEmployeesByCompany))]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByCompany(Guid companyId,[FromQuery] EmployeeDtoParameters parameters)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            if (!_propertyCheckerServeice.TypeHasProperties<EmployeeDto>(parameters.Fields))
            {
                return NotFound();
            }
            var employees = await _companyRepository.GetEmployeesAsync(companyId, parameters);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeesDto.ShapeData(parameters.Fields));
        }
        [HttpGet(template:"{employeeId}",Name =nameof(GetEmployeeByCompany))]
        //[ResponseCache(Duration = 60)]//只能设置缓存header，但是没有任何缓存的能力
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1800)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByCompany(Guid companyId,Guid employeeId,string fields)
        {
            if (!_propertyCheckerServeice.TypeHasProperties<EmployeeDto>(fields))
            {
                return NotFound();
            }
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var employee = await _companyRepository.GetEmployeeAsync(companyId,employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return Ok(employeeDto.ShapeData(fields));
        }
        [HttpPost(Name = nameof(CreateEmployeeForCompany))]
        public async Task<ActionResult<EmployeeDto>> CreateEmployeeForCompany(Guid companyId, EmployeeAddDto employee)
        {
            if(!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var entity = _mapper.Map<Employee>(employee);
            _companyRepository.AddEmployee(companyId, entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<EmployeeDto>(entity);
            return CreatedAtRoute(nameof(GetEmployeeByCompany), routeValues: new 
            { 
                companyId ,
                employeeId = returnDto.Id
            }
            ,value: returnDto);
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId,Guid employeeId, EmployeeUpdateDto employee) 
        {
            if(!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId,employeeId);
            if (employeeEntity == null)
            {
                var employeeToAddEntity = _mapper.Map<Employee>(employee);
                employeeToAddEntity.Id = employeeId;
                _companyRepository.AddEmployee(companyId, employeeToAddEntity);
                await _companyRepository.SaveAsync();
                var returnDto = _mapper.Map<EmployeeDto>(employeeToAddEntity);
                return CreatedAtRoute(nameof(GetEmployeeByCompany)
                    ,new { 
                        companyId,
                        employeeId = employeeId
                    },employeeToAddEntity);
            }
            //entity 转化为 updateDto
            //把传进来的employee的值更新到 updateDto
            //把updateDto映射会entity
            _mapper.Map(employee,employeeEntity);

            _companyRepository.UpdateEmployee(employeeEntity);
            await _companyRepository.SaveAsync();
            return NoContent();
        }
        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId,Guid employeeId,JsonPatchDocument<EmployeeUpdateDto> patchDocument)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employeeEntity == null)
            {
                var employeeDto = new EmployeeUpdateDto();
                patchDocument.ApplyTo(employeeDto,ModelState);
                if (!TryValidateModel(employeeDto))
                {
                    return ValidationProblem();
                }
                var employeeToAdd = _mapper.Map<Employee>(employeeDto);
                employeeToAdd.Id = employeeId;

                _companyRepository.AddEmployee(companyId, employeeToAdd);
                await _companyRepository.SaveAsync();
                var dtoToReturn = _mapper.Map<EmployeeDto>(employeeToAdd);
                return CreatedAtRoute(nameof(GetEmployeeByCompany),new { companyId = companyId,employeeId = dtoToReturn.Id},dtoToReturn);
            }
            var dtoToPatch = _mapper.Map<EmployeeUpdateDto>(employeeEntity);

            //需要处理验证错误
            patchDocument.ApplyTo(dtoToPatch, ModelState);
            if (TryValidateModel(dtoToPatch))
            {
                return ValidationProblem(ModelState);
            };

            _mapper.Map(dtoToPatch,employeeEntity);
            _companyRepository.UpdateEmployee(employeeEntity);
            await _companyRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId,Guid employeeId)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var employee = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            _companyRepository.DeleteEmployee(employee);
            await _companyRepository.SaveAsync();
            return NoContent();

        }
        public override ActionResult ValidationProblem(ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}