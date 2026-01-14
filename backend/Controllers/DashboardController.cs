using backend.Interfaces;
using backend.Models;
using backend.Services;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardsController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Veilingsmeester")]
        public async Task<IActionResult> GetAll()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Veilingsmeester"))
            {
                return Forbid();
            }

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