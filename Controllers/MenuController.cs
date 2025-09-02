using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.DTOs.MenuItemDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _menuService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _menuService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] MenuItemCreateDTO menuItemDto)
        {
            var newItemId = await _menuService.AddItemAsync(menuItemDto);
            return CreatedAtAction(nameof(GetById), new { id = newItemId }, new { Id = newItemId });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] MenuItemGetDTO menuItemGetDTO)
        { 
            var findItem = await _menuService.UpdateItemAsync(menuItemGetDTO);
            if(!findItem)
                return NotFound("Menu item not found , update failed");
            return NoContent();
        }
    }
}
