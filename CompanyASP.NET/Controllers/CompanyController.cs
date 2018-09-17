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
            var result = Repository.RetrieveAll();
            return result.Count() != 0 ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        }

        // GET api/company/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = Repository.Retrieve(id);
            return result != null ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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
                    // Log this here?
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
        [HttpPost("collection")]
        public IActionResult Post([FromBody] IEnumerable<Company> list)
        {
            List<int> result;
            result = Repository.Create(list).ToList();
            return result.Count > 0 ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status422UnprocessableEntity);
        }

        // PUT api/company/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Company value)
        {
            value.Id = id;
            bool success = Repository.Update(value);
            return success ? Ok() : (IActionResult)BadRequest("Could not be updated");
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