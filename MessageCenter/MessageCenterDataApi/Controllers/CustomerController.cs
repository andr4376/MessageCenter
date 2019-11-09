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
    public class CustomerController : ApiController
    {
        // GET: api/Customer
        [Route("api/Customers")]

        public List<Customer> Get()
        {
            return Database.Instance.GetAllCustomers();
        }

        [Route("api/Customers/cpr/{cpr}")]
        [HttpGet]
        public List<Customer> Get(string cpr)
        {
            List<Customer> tmpCustomer = new List<Customer>() { Database.Instance.GetCustomer(cpr) };

            return tmpCustomer;
        }

    }
}
