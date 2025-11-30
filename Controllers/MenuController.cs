using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
        private readonly ILogger <MenuController> _logger;
        public MenuController(IMenuService menuService, ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }
        [AllowAnonymous]
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
                _logger.LogWarning("Menu item with ID {Id}", id);
                return NotFound();
            }
            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] MenuItemCreateDTO menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newItemId = await _menuService.AddItemAsync(menuItemDto);
            return CreatedAtAction(nameof(GetById), new { id = newItemId }, new { Id = newItemId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] MenuItemUpdateDTO menuItemUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(id != menuItemUpdateDto.Id) return BadRequest("Id in URL does not match Id in body");

            var findItem = await _menuService.UpdateItemAsync(menuItemUpdateDto);
            if(!findItem)
                return NotFound("Menu item not found , update failed");
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var isDeleted = await _menuService.DeleteItemAsync(id);
            if (!isDeleted)
            {
                return NotFound("Menu item not found, delete failed");
            }
            return NoContent();
        }
    }
}
