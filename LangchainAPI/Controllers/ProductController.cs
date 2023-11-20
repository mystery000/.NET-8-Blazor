using LangchainAPI.Helpers;
using LangchainAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LangchainAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly ProductContext _productContext;

        public ProductController(ProductContext productContext)
        {
            _productContext = productContext;
        }

        [HttpGet]
        public async Task<String> Test()
        {
            await Task.Delay(500);
            return "Langchain Integration Test";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (_productContext.Products == null)
            {
                return NotFound();
            }
            return await _productContext.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_productContext.Products == null)
            {
                return NotFound();
            }
            var product = await _productContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _productContext.Products.Add(product);
            await _productContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            _productContext.Entry(product).State = EntityState.Modified;
            try
            {
                await _productContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_productContext.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_productContext.Products == null)
            {
                return NotFound();
            }
            var product = await _productContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _productContext.Products.Remove(product);
            await _productContext.SaveChangesAsync();
            return NoContent();
        }

    }
}
