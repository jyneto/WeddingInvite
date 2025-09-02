using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class TableController : ControllerBase   
    {
        private readonly ITableService _tableService;
        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }
        [HttpGet]
        public async Task<IActionResult> GetTable()
        {
            var tables = await _tableService.GetAllTableAsync();
            return Ok(tables);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            var table = await _tableService.GetTableByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            return Ok(table);
        }

        [HttpPost]
        public async Task<IActionResult> AddTable([FromBody] TableGetDTO tableDto)
        {
            var newTableId = await _tableService.AddTableAsync(tableDto);
            return CreatedAtAction(nameof(GetTableById), new { id = newTableId }, new { Id = newTableId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTable([FromBody] TableUpdateDTO tableDto)
        {
            var isUpdated = await _tableService.UpdateTableAsync(tableDto);
            if (!isUpdated)
            {
                return NotFound("Table not found , update failed");
            }
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var isDeleted = await _tableService.DeleteTableAsync(id);
            if (!isDeleted)
            {
                return NotFound("Table not found , delete failed");
            }
            return NoContent();
        }
    }
}
