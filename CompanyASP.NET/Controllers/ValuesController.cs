using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CompanyASP.NET.Repository;
using CompanyASP.NET.Models;
using Newtonsoft.Json;

namespace CompanyASP.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> Get()
        {
                return new ActionResult<IEnumerable<Employee>>(EmployeeRepository.getInstance().RetrieveAll());
            //IEnumerable<Employee> list = 
            //return list;
            //return JsonConvert.SerializeObject(list);
            /*ist<string> result = new List<string>();
            foreach(Employee e in list)
            {
                result.Add(JsonConvert.SerializeObject(e));
            }
            return result;*/
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
