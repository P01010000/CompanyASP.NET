﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CompanyASP.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private IRepository<Department> Repository;
        public DepartmentController(IRepository<Department> repository)
        {
            Repository = repository;
        }

        // GET api/employee
        [HttpGet]
        public IActionResult Get([FromQuery] int? cid)
        {
            var result = cid.HasValue ? Repository.RetrieveAll(cid.Value) : Repository.RetrieveAll();
            return result.Count() > 0 ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        }

        // GET api/employee/5
        [HttpGet("{did}")]
        public IActionResult Get(int did, [FromQuery] int? cid)
        {
            var result = cid.HasValue ? Repository.Retrieve(did, cid.Value) : Repository.Retrieve(did);
            return result != null ? Ok(result) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        }

        // POST api/employee
        [HttpPost]
        public IActionResult Post([FromBody] Department value)
        {
            int id = Repository.Create(value);
            return id > 0 ? Ok(id) : (IActionResult)BadRequest("Could not be created");
        }

        // PUT api/employee/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Department value)
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