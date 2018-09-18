using Chayns.Backend.Api.Credentials;
using Chayns.Backend.Api.Credentials.Base;
using Chayns.Backend.Api.Repositories;
using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TobitWebApiExtensions.Extensions;

namespace CompanyASP.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IRepository<Employee> Repository;
        public EmployeeController(IRepository<Employee> repository)
        {
            Repository = repository;
        }

        // GET api/employee
        [HttpGet]
        public IActionResult Get()
        {
            // Reading uac groups and payload
            // var pl = HttpContext.GetTokenPayload();
            // var uac = HttpContext.GetUacGroups();

            try
            {
                var result = Repository.RetrieveAll();
                return Ok(result);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case RepositoryErrorType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }

        // GET api/employee/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = Repository.Retrieve(id);
                return Ok(result);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case RepositoryErrorType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }

        // POST api/employee
        [HttpPost]
        [Authorize(Roles = "Api, 1")]
        public IActionResult Post([FromBody] Employee value)
        {
            try
            {
                int id = Repository.Create(value);
                return Ok(id);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case RepositoryErrorType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }

        // POST api/employee/collection
        [HttpPost("collection")]
        [Authorize(Roles = "Api, 1")]
        public IActionResult Post([FromBody] IEnumerable<Employee> list)
        {
            try
            {
                List<int> result;
                result = Repository.Create(list).ToList();
                return Ok(result);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case RepositoryErrorType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }

        // PUT api/employee/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Api, 1")]
        public IActionResult Put(int id, [FromBody] Employee value)
        {
            try
            {
                value.Id = id;
                bool success = Repository.Update(value);
                return Ok(Repository.Retrieve(id));
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case RepositoryErrorType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }

        // DELETE api/employee/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Api, 1")]
        public IActionResult Delete(int id)
        {
            try
            {
                bool success = Repository.Delete(id);
                return success ? Ok(1) : Ok(0);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case RepositoryErrorType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }
    }
}