using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Services;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ControllerBase
    {
        private readonly IDashboardController _service;

        public DashboardsController(IDashboardController service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dashboard = await _service.GetByIdAsync(id);
            if (dashboard == null) return NotFound();
            return Ok(dashboard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDashboard(int id, Dashboard dashboard)
        {
            var updated = await _service.UpdateAsync(id, dashboard);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostDashboard(Dashboard dashboard)
        {
            var created = await _service.AddAsync(dashboard);
            return CreatedAtAction("GetById", new { id = created.DashboardID }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDashboard(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}