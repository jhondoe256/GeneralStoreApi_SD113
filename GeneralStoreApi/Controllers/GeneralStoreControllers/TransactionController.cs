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
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();


        //Create(POST)
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Transaction transaction)
        {
            if (transaction is null)
            {
                return BadRequest("Invalid data entry");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //we need to check for our customer b/c its a required ForeignKey 
            var customer = await _context.Customers.FindAsync(transaction.CustomerID);
            if (customer is null)
            {
                return BadRequest("Invalid Customer Info");
            }

            //we can focus on the Product
            var product = await _context.Products.FindAsync(transaction.ProductSKU);
            if (product is null)
            {
                return BadRequest("Invalid Product Info");
            }
            //Verify that the product is in stock
            if (product.NumberInInventory==0)
            {
                return BadRequest($"{product.Name} is out of stock.");
            }

            if (!product.IsNull)
            {
                var negativeValueCheck = product.NumberInInventory - transaction.ItemCount;
                //Check that there is enough product to complete the Transaction
                if (negativeValueCheck<0)
                {
                    return BadRequest($"Sorry, there are only {product.NumberInInventory} items in stock.");
                }
                else
                {
                    //Remove the Products that were bought
                    product.NumberInInventory -= transaction.ItemCount;
                }
            }

            transaction.DateOfTransaction = DateTime.Now;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok($"Transaction number: {transaction.ID} was successfully created." +
                                           $"Product inventory left {product.NumberInInventory} " +
                                           $"{customer.FirstName}, bought {transaction.ItemCount} of {product.Name}!");
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions()
        {
            var trasactions = await _context.Transactions.Include(t=>t.Customer).Include(t=>t.Product).ToListAsync();
            return Ok(trasactions);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionByID(int id)
        {
            //we did not use the 'virtual' keyword so... what do we do....
            var transaction = await _context.Transactions
                                                        .Include(t => t.Customer) //including customer data
                                                        .Include(t => t.Product) //including product data
                                                        .SingleAsync(t => t.ID == id);
            return Ok(transaction);
        }
        //get all transactions by the customerId
        [HttpGet]
        [Route("api/Transaction/CustomerTransactions/{id}")]
        public async Task<IHttpActionResult> GetTransactionsByCustomerID(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null)
            {
                return NotFound();
            }
            var transactions = await _context.Transactions.Include(t=>t.Product).Where(t => t.CustomerID == customer.ID).ToListAsync();
            return Ok(transactions);
        }
        //total product sales 
        [HttpGet]
        [Route("api/Transaction/TotalProductSales/{productSku}")]
        public async Task<IHttpActionResult> GetTotalProductSales(string productSku)
        {
            var totalValue = 0.0;
            var products = await _context.Transactions
                                                      .Include(t => t.Product)
                                                      .Where(t => t.ProductSKU == productSku)
                                                      .ToListAsync();
            foreach (var item in products)
            {
                totalValue += item.ItemCount * item.Product.Cost;
            }
            return Ok(Math.Round(totalValue, 2));
        }
    }
}
