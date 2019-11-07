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
        public Employee Get(string tuser)
        {
            Employee tmpEmployee = Database.Instance.GetEmployee(tuser);

            return tmpEmployee != null ? tmpEmployee : new Employee() { FirstName = "Medarbejder findes ikke" };
        }
    }
}
