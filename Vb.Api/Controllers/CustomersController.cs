using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vb.Data;
using Vb.Data.Entity;

namespace VbApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly VbDbContext dbContext;

    public CustomersController(VbDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<List<Customer>> Get()
    {
        return await dbContext.Set<Customer>()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<Customer> Get(int id)
    {
        var customer =  await dbContext.Set<Customer>()
            .Include(x=> x.Accounts)
            .Include(x=> x.Addresses)
            .Include(x=> x.Contacts)
            .Where(x => x.Id == id).FirstOrDefaultAsync();
       
        return customer;
    }

    [HttpPost]
    public async Task Post([FromBody] Customer customer)
    {
        await dbContext.Set<Customer>().AddAsync(customer);
        await dbContext.SaveChangesAsync();
    }

    [HttpPut("{id}")]
    public async Task Put(int id, [FromBody] Customer customer)
    {
        var fromdb = await dbContext.Set<Customer>().Where(x => x.Id == id).FirstOrDefaultAsync();
        fromdb.FirstName = customer.FirstName;
        fromdb.LastName = customer.LastName;

        await dbContext.SaveChangesAsync();
    }

    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        var fromdb = await dbContext.Set<Customer>().Where(x => x.Id == id).FirstOrDefaultAsync();
        fromdb.IsActive = false;
        await dbContext.SaveChangesAsync();
    }
}