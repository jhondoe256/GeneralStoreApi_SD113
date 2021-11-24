using GeneralStoreApi.Models;
using GeneralStoreApi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreApi.Controllers.GeneralStoreControllers
{
    public class CustomerController : ApiController
    {
        //so now we need to use our dbContext....Its now called ApplicationDbContext
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //So implement crud
        //Create
        [HttpPost]
        public async Task<IHttpActionResult> AddCustomer(Customer customer)
        {
            //want to check if customer is null
            if (customer is null)
            {
                return BadRequest("Invalid data entry.");
            }
            //check the Modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //add to the database
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return Ok($"{customer.FirstName} {customer.LastName} was added to the database.");
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerByID(int id)
        {
            if (id<1)
            {
                return BadRequest("Invalid data entry.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer(int id, Customer updatedCustomerData)
        {
            //make sure that the ids match
            if (id!=updatedCustomerData.ID)
            {
                return BadRequest("Ids dont match");
            }
            //make sure updatedCustomerData is not null
            if (updatedCustomerData is null)
            {
                return BadRequest("Invalid data entry");
            }
            //check the modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //check to see if customer exist
            var customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            //lastly make needed changes...
            customer.FirstName = updatedCustomerData.FirstName;
            customer.LastName = updatedCustomerData.LastName;

            //save changes
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            //find the customer
            //_context.Customers is an IEnumerable
            //So, we can use linq to do our queries
            //iterate through all of the customers and 
            //see if one exist -> a customer with the same ID as 
            // whats being passed in via the method
            var customer = await _context.Customers.SingleOrDefaultAsync(x => x.ID == id);

            if (customer is null)
            {
                return NotFound();
            }

            //delete em
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok($"{customer.FirstName}, {customer.LastName} has been deleted.");
        }
    }
}
