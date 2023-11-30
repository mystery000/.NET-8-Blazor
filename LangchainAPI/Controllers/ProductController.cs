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

        [HttpGet("{query}")]
        public async Task<ActionResult<string>> Search(string query)
        {
            object response = PythonInterop.RunPythonCodeAndReturn(
 @"from langchain.agents import create_sql_agent
from langchain.agents.agent_toolkits import SQLDatabaseToolkit
from langchain.agents.agent_types import AgentType
from langchain.llms.openai import OpenAI
from langchain.sql_database import SQLDatabase

API_KEY = ""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""

# Setup database
db = SQLDatabase.from_uri(
    f""postgresql+psycopg2://postgres:postgres@localhost:5432/products"",
)

# setup llm
llm = OpenAI(temperature=0, openai_api_key=API_KEY)

# Create db chain
QUERY = """"""
Given an input question, first create a syntactically correct postgresql query to run, then look at the results of the query and return the answer.
Use the following format:

Question: Question here
SQLQuery: SQL Query to run
SQLResult: Result of the SQLQuery
Answer: Final answer here

{question}
""""""

toolkit = SQLDatabaseToolkit(db=db, llm=llm)

# Setup the database chain
agent_executor = create_sql_agent(llm=llm, verbose=True, toolkit=toolkit, agent_type=AgentType.ZERO_SHOT_REACT_DESCRIPTION)
prompt = query
question = QUERY.format(question=prompt)
response = agent_executor.run(question)
", query, "query", "response");
            return response.ToString();
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
