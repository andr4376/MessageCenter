using MessageCenterDataApi.Code;
using MessageCenterDataApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MessageCenterDataApi.Controllers
{
    public class EmployeeController : ApiController
    {
        // GET: api/Employees
        [Route("api/Employees")]
        public List<Employee> Get()
        {
            return Database.Instance.GetAllEmployees();
        }

        [Route("api/Employees/tuser/{tuser}")]
        [HttpGet]
        public List<Employee> Get(string tuser)
        {
            List<Employee> tmpEmployee = new List<Employee>() { Database.Instance.GetEmployee(tuser) };

            return tmpEmployee;
        }
        [Route("api/Employees/login/{tuser}/{password}")]
        [HttpGet]
        public List<Employee> Get(string tuser,string password)
        {
            List<Employee> tmpEmployee = new List<Employee>() { Database.Instance.GetEmployee(tuser,password) };

            return tmpEmployee;
        }
    }
}
