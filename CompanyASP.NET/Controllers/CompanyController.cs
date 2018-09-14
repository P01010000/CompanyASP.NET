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

        // GET api/employee
        [HttpGet]
        public IActionResult Get()
        {
            var result = Repository.RetrieveAll();
            return result.Count() != 0 ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        }

        // GET api/employee/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = Repository.Retrieve(id);
            return result != null ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        }

        // POST api/employee
        [HttpPost]
        public IActionResult Post([FromBody] Company value)
        {
            int id = 0;
            try
            {
                id = Repository.Create(value);
            } catch(RepositoryException<CreateResultType> ex)
            {
                switch (ex.Type)
                {
                    // Log this here?
                    case CreateResultType.OK:
                        return Ok();
                    case CreateResultType.INVALID_ARGUMENT:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                    case CreateResultType.NOT_FOUND:
                        return StatusCode(StatusCodes.Status204NoContent, ex.Message);
                    case CreateResultType.SQL_EXCEPTION:
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    case CreateResultType.NOT_INSERTED:
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
                }
            }
            return id > 0 ? Ok(id) : (IActionResult)BadRequest("Could not be created");
        }

        // PUT api/employee/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Company value)
        {
            bool success = Repository.Update(value);
            return success ? Ok() : (IActionResult)BadRequest("Could not be updated");
        }

        // DELETE api/employee/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = Repository.Delete(id);
            return success ? Ok(1) : Ok(0);
        }
    }
}