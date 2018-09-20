using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyASP.NET.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyASP.NET.Controllers
{
    [Route("api/[controller]")]
    public class UacController : Controller
    {
        private IUacHelper _uacHelper;
        public UacController(IUacHelper uacHelper)
        {
            _uacHelper = uacHelper;
        }

        // GET: Uac
        [HttpGet("{locationId}/{userId}")]
        public IActionResult Get(int locationId, int userId)
        {
            var result = _uacHelper.GetUacGroups(locationId, userId);
            return Ok(result);
        }
    }
}