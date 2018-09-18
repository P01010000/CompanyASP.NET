using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TobitWebApiExtensions.Extensions;

namespace CompanyASP.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private IRepository<Company> Repository;
        public CompanyController(IRepository<Company> repository)
        {
            Repository = repository;
        }

        // GET api/company
        [HttpGet]
        public IActionResult Get()
        {
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

        // GET api/company/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Company result;
            try
            {
                result = Repository.Retrieve(id);
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

        // POST api/company
        [HttpPost]
        public IActionResult Post([FromBody] Company value)
        {
            int id = 0;
            try
            {
                id = Repository.Create(value);
                return Ok(id);
            } catch(RepositoryException<RepositoryErrorType> ex)
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

        // POST api/company/collection
        [HttpPost("collection")]
        public IActionResult Post([FromBody] IEnumerable<Company> list)
        {
            List<int> result;
            try
            {
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

        // PUT api/company/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Company value)
        {
            try
            {
                value.Id = id;
                bool success = Repository.Update(value);
                return Ok(success);
            }
            catch (RepositoryException<RepositoryErrorType> ex)
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

        // DELETE api/company/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = Repository.Delete(id);
            return success ? Ok(1) : Ok(0);
        }
    }
}