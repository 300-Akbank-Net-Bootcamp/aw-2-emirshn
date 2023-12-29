using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vb.Data;

namespace VbApi.Controllers;

public class Staff
{
    public int Id { get; set; }

    [Required]
    [StringLength(maximumLength: 250, MinimumLength = 10)]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Phone is not valid.")]
    public string? Phone { get; set; }

    [Range(minimum: 30, maximum: 400, ErrorMessage = "Hourly salary does not fall within allowed range.")]
    public decimal? HourlySalary { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly VbDbContext dbContext;

    public StaffController(VbDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<List<Staff>> Get()
    {
        return await dbContext.Set<Staff>().ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<Staff> Get(int id)
    {
        var staff = await dbContext.Set<Staff>().FindAsync(id);
        return staff;
    }

    [HttpPost]
    public async Task<Staff> Post([FromBody] Staff staff)
    {
        if (ModelState.IsValid)
        {
            await dbContext.Set<Staff>().AddAsync(staff);
            await dbContext.SaveChangesAsync();
        }
        return staff;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Staff staff)
    {
        if (id != staff.Id)
        {
            return BadRequest();
        }

        var fromdb = await dbContext.Set<Staff>().Where(x => x.Id == id).FirstOrDefaultAsync();

        if (fromdb == null)
        {
            return NotFound();
        }

        fromdb.Name = staff.Name;
        fromdb.Email = staff.Email;
        fromdb.Phone = staff.Phone;
        fromdb.HourlySalary = staff.HourlySalary;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var staff = await dbContext.Set<Staff>().FindAsync(id);

        if (staff == null)
        {
            return NotFound();
        }

        dbContext.Set<Staff>().Remove(staff);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

}
