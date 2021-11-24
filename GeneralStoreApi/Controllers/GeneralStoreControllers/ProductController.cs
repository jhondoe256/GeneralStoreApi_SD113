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
    public class ProductController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Crud...
        [HttpPost]
        public async Task<IHttpActionResult> AddProduct([FromBody] Product product)
        {
            //want to check if customer is null
            if (product is null)
            {
                return BadRequest("Invalid data entry.");
            }
            //check the Modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //add to the database
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok($"{product.Name} was added to the database.");
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [HttpGet]
        [Route("api/Product/{SKU}")]
        public async Task<IHttpActionResult> GetProductBySKU(string SKU)
        {
            var product = await _context.Products.FindAsync(SKU);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPut]
        [Route("api/Product/UpdateProduct/{SKU}")]
        public async Task<IHttpActionResult> UpdateProduct(string SKU, Product updatedProductData)
        {
            //make sure that the ids match
            if (SKU != updatedProductData.SKU)
            {
                return BadRequest("Ids dont match");
            }
            //make sure updatedProductData is not null
            if (updatedProductData is null)
            {
                return BadRequest("Invalid data entry");
            }
            //check the modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //check to see if product exist
            var product = await _context.Products.FindAsync(SKU);

            if (product is null)
            {
                return NotFound();
            }

            //lastly make needed changes...
            product.Name = updatedProductData.Name;
            product.NumberInInventory = updatedProductData.NumberInInventory;
            product.Cost = updatedProductData.Cost;

            //save changes
            await _context.SaveChangesAsync();

            return Ok($"{product.Name} was successfully updated.");
        }
        [HttpDelete]
        [Route("api/Product/DeleteProduct/{SKU}")]
        public async Task<IHttpActionResult> DeleteProduct(string SKU)
        {
            var product = await _context.Products.FindAsync(SKU);
            if (product is null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok($"{product.Name} was successfully deleted.");
        }
    }
}
