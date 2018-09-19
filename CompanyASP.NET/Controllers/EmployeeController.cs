using Auth.Models;
using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using TobitLogger.Core;
using TobitLogger.Core.Models;
using TobitWebApiExtensions.Extensions;

namespace CompanyASP.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IRepository<Employee> _repository;
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(IRepository<Employee> repository, ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _logger = loggerFactory.CreateLogger<EmployeeController>();
        }

        // GET api/employee
        [HttpGet]
        public IActionResult Get()
        {
            // Reading uac groups and payload
            // var pl = HttpContext.GetTokenPayload();
            // var uac = HttpContext.GetUacGroups();
            // var payload = HttpContext.GetTokenPayload<LocationUserTokenPayload>();
            // if (payload == null || payload.LocationId != 157669) return StatusCode(StatusCodes.Status403Forbidden, "Token not valid for this location");
            try
            {
                var result = _repository.RetrieveAll();
                return Ok(result);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        var logObj = new ExceptionData(ex);
                        logObj.AddCustomText(ex.Message);
                        _logger.Error(logObj);
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case RepositoryErrorType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case RepositoryErrorType.SQL_EXCEPTION:
                        _logger.Error(new ExceptionData(ex));
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
                var result = _repository.Retrieve(id);
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
            var payload = HttpContext.GetTokenPayload<LocationUserTokenPayload>();
            if (payload == null || payload.LocationId != 157669)
            {
                _logger.Warning("Used token is not valid for this location. " + payload.LocationId);
                return StatusCode(StatusCodes.Status403Forbidden, "Token not valid for this location");
            }
            try
            {
                int id = _repository.Create(value);
                return Ok(id);
            } catch (RepositoryException<RepositoryErrorType> ex)
            {
                switch (ex.Type)
                {
                    case RepositoryErrorType.INVALID_ARGUMENT:
                        var logObj = new ExceptionData(ex);
                        logObj.Add("newEmployee", value);
                        logObj.AddCustomText(ex.Message);
                        _logger.Error(logObj);
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
            var payload = HttpContext.GetTokenPayload<LocationUserTokenPayload>();
            if (payload == null || payload.LocationId != 157669) return StatusCode(StatusCodes.Status403Forbidden, "Token not valid for this location");
            try
            {
                List<int> result;
                result = _repository.Create(list).ToList();
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
                bool success = _repository.Update(value);
                return Ok(_repository.Retrieve(id));
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
                bool success = _repository.Delete(id);
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