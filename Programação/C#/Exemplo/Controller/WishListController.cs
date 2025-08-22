using AutoMapper;
using backend_wishlist.Data;
using backend_wishlist.DTO;
using backend_wishlist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace wishlist.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WishlistController : ControllerBase
{
    private readonly WishlistContext _context;
    private readonly IMapper _mapper;

    #region Constructor
    public WishlistController(WishlistContext context, IMapper mapper)
    {
        _context = context; // Injeção de dependência para o BANCO DE DADOS
        _mapper = mapper;   // Injeção de dependência para o AUTO MAPPER
    }
    #endregion

    #region GET/GetItems
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _context.WishLists.ToListAsync();
        return Ok(items);
    }
    #endregion

    #region GET/GetById
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var existingItem = await _context.WishLists.FindAsync(id);

        if(existingItem == null)
        {
            return NotFound("Item não encontrado...");
        }

        return Ok(existingItem);
    }
    #endregion

    #region POST/AddNewItem
    [HttpPost]
    public async Task<IActionResult> AddNewItem([FromBody] WishlistCreate item)
    {
        if (item == null)
        {
            return BadRequest("Item inválido.");
        }

        // Mapeando o DTO para a entidade WishList com o AutoMapper
        var wishListItem = _mapper.Map<WishList>(item);

        _context.WishLists.Add(wishListItem);

        await _context.SaveChangesAsync();

        return Created("Seu item foi adicionado a lista de desejos!", item);
    }
    #endregion

    #region PUT/UpdateById
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateById(long id, [FromBody] WishlistCreate updatedItem)
    {
        if (updatedItem == null)
        {
            return BadRequest("Dados inválidos!");
        }

        var existingItem = await _context.WishLists.FindAsync(id);

        if (existingItem == null)
        {
            return NotFound("Seu item de desejo não foi encontrado...");
        }

        _mapper.Map(updatedItem, existingItem);

        await _context.SaveChangesAsync();

        return Ok(existingItem);
    }
    #endregion

    #region DELETE/DeleteItem
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(long id)
    {
        var existingItem = await _context.WishLists.FindAsync(id);

        if (existingItem == null)
        {
            return NotFound("Item não encontrado.");
        }

        _context.WishLists.Remove(existingItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion
}
