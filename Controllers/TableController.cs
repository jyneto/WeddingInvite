using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvite.Api.DTOs.TableDTO;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Controllers
{
    [ApiController]
    [Route("api/tables")]
    [Authorize(Roles = "Admin")]
    public class TableController : ControllerBase   
    {
        private readonly ITableService _tableService;
        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTables()
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
        public async Task<IActionResult> AddTable([FromBody] TableCreateDTO tableDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var newTableId = await _tableService.AddTableAsync(tableDto);
            return CreatedAtAction(nameof(GetTableById), new { id = newTableId }, new { Id = newTableId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] TableUpdateDTO tableDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != tableDto.Id)
                return BadRequest("ID mismatch");

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
            try
            {
                var ok = await _tableService.DeleteTableAsync(id);
                if (!ok) return NotFound();            
                return NoContent();                   
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);          
            }
        }

    }
}
