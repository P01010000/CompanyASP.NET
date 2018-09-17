using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Post([FromBody] Employee value)
        {
            int id = Repository.Create(value);
            return id > 0 ? Ok(id) : (IActionResult)BadRequest("Could not be created");
        }

        // POST api/employee
        [HttpPost("collection")]
        public IActionResult Post([FromBody] IEnumerable<Employee> list)
        {
            List<int> result;
            result = Repository.Create(list).ToList();
            return result.Count > 0 ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status422UnprocessableEntity);
        }

        // PUT api/employee/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Employee value)
        {
            value.Id = id;
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