using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private TabloidDbContext _context;

    public CategoryController(TabloidDbContext context)
    {
        _context = context;
    }

    //GET all categories
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Get()
    {
        var categories = _context.Categories.OrderBy(c => c.Name);
        return Ok(categories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }));
    }

    //GET a single category
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetById(int id)
    {
        var category = _context.Categories.SingleOrDefault(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        var categoryDTO = new CategoryDTO { Id = category.Id, Name = category.Name };
        return Ok(categoryDTO);
    }

    //POST a new category
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Post([FromBody] CategoryDTO categoryDTO)
    {
        var category = new Category { Name = categoryDTO.Name };

        _context.Categories.Add(category);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryDTO);
    }

    //DELETE a category
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var category = _context.Categories.SingleOrDefault(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return NoContent();
    }

    //PUT a category
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Put(int id, [FromBody] CategoryDTO categoryDTO)
    {
        var category = _context.Categories.SingleOrDefault(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        category.Name = categoryDTO.Name;
        _context.SaveChanges();
        return NoContent();
    }
}
