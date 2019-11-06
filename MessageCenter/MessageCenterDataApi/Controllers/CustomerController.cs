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
        List<Customer> tmpList = new List<Customer>();


        // GET: api/Customer
        [Route("api/Customers")]

        public List<Customer> Get()
        {
            return Database.Instance.GetAllCustomers();
        }

        [Route("api/Customers/cpr/{cpr}")]
        [HttpGet]
        public Customer Get(string cpr)
        {
            Customer tmpCustomer = Database.Instance.GetCustomer(cpr);

            return tmpCustomer != null ? tmpCustomer : new Customer() { FirstName = "Kunde findes ikke" };
        }

    }
}
