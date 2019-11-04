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

        public CustomerController()
        {
           
        }
        // GET: api/Customer
        public List<Customer> Get()
        {
            return tmpList;
        }

        // GET: api/Customer/5
        public Customer Get(int id)
        {
            return new Customer();
        }


        [Route("api/Customer/cpr/{cpr}")]
        [HttpGet]
        public Customer Get(string cpr)
        {
            Customer tmpCustomer = Customer.GetCustomers.
                Where(customer => customer.Cpr == cpr).FirstOrDefault();
                       

          return  tmpCustomer!=null?tmpCustomer:new Customer();
        }

        // POST: api/Customer
        public void Post([FromBody]string value)
        {
        }



        // DELETE: api/Customer/5
        public void Delete(int id)
        {
        }
    }
}
