using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vb.Data;

namespace VbApi.Controllers;

public class Employee : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(maximumLength: 250, MinimumLength = 10, ErrorMessage = "Invalid Name")]
    public string Name { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Phone is not valid.")]
    public string Phone { get; set; }

    [Range(minimum: 50, maximum: 400, ErrorMessage = "Hourly salary does not fall within allowed range.")]
    [MinLegalSalaryRequired(minJuniorSalary: 50, minSeniorSalary: 200)]
    public double HourlySalary { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var minAllowedBirthDate = DateTime.Today.AddYears(-65);
        if (minAllowedBirthDate > DateOfBirth)
        {
            yield return new ValidationResult("Birthdate is not valid.");
        }
    }
}

public class MinLegalSalaryRequiredAttribute : ValidationAttribute
{
    public MinLegalSalaryRequiredAttribute(double minJuniorSalary, double minSeniorSalary)
    {
        MinJuniorSalary = minJuniorSalary;
        MinSeniorSalary = minSeniorSalary;
    }

    public double MinJuniorSalary { get; }
    public double MinSeniorSalary { get; }
    public string GetErrorMessage() => $"Minimum hourly salary is not valid.";

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        var employee = (Employee)validationContext.ObjectInstance;
        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirdyYears = employee.DateOfBirth <= dateBeforeThirtyYears;
        var hourlySalary = (double)value;

        var isValidSalary = isOlderThanThirdyYears ? hourlySalary >= MinSeniorSalary : hourlySalary >= MinJuniorSalary;

        return isValidSalary ? ValidationResult.Success : new ValidationResult(GetErrorMessage());
    }
}

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly VbDbContext dbContext;

    public EmployeeController(VbDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<List<Employee>> Get()
    {
        return await dbContext.Set<Employee>().ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<Employee> Get(int id)
    {
        var employee = await dbContext.Set<Employee>().FindAsync(id);
        return employee;
    }

    [HttpPost]
    public async Task<Employee> Post([FromBody] Employee employee)
    {
        if (ModelState.IsValid)
        {
            await dbContext.Set<Employee>().AddAsync(employee);
            await dbContext.SaveChangesAsync();
        }
        return employee;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        var fromdb = await dbContext.Set<Employee>().Where(x => x.Id == id).FirstOrDefaultAsync();

        if (fromdb == null)
        {
            return NotFound();
        }

        fromdb.Name = employee.Name;
        fromdb.DateOfBirth = employee.DateOfBirth;
        fromdb.Email = employee.Email;
        fromdb.Phone = employee.Phone;
        fromdb.HourlySalary = employee.HourlySalary;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await dbContext.Set<Employee>().FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        dbContext.Set<Employee>().Remove(employee);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

}
