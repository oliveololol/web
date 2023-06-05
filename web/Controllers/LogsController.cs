using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Models;

namespace web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly userContext _context;

        public LogsController(userContext context)
        {
            _context = context;
        }

        // GET: api/Logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLog()
        {
            return await _context.Log.ToListAsync();
        }

        // GET: api/Logs/5
        [HttpGet("id")]
        public async Task<ActionResult<Log>> GetLog(int id)
        {
            var log = await _context.Log.FindAsync(id);
           
            if (log == null)
            {
                return NotFound();
            }

            return log;
        }
        [HttpGet("log/{Id_Users}")]
        public async Task<ActionResult<IEnumerable<Log>>> logs(int Id_Users )
        {
            var Logs = await _context.Log.Include(l => l.IdEventNavigation)
                .Where(l => l.IdUsers == Id_Users)
                .ToListAsync();
            
            return Logs;
        }

        // PUT: api/Logs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLog(int id, Log log)
        {
            if (id != log.IdUsers)
            {
                return BadRequest();
            }

            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Logs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Log>> PostLog(Log log)
        {
            _context.Log.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLog", new { id = log.IdUsers }, log);
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Log>> DeleteLog(int id)
        {
            var log = await _context.Log.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.Log.Remove(log);
            await _context.SaveChangesAsync();

            return log;
        }

        private bool LogExists(int id)
        {
            return _context.Log.Any(e => e.IdUsers == id);
        }
    }
}
