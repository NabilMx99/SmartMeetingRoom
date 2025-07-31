using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/actionitems")]
    [ApiController]
    public class ActionItemController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;

        public ActionItemController(SmartMeetingRoomDBContext context)
        {
            _context = context;
        }

        // GET: api/actionitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActionItem>>> GetActionItems()
        {
            return await _context.ActionItems.ToListAsync();
        }

        // GET: api/actionitems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ActionItem>> GetActionItem(int id)
        {
            var actionItem = await _context.ActionItems.FindAsync(id);

            if (actionItem == null)
            {
                return NotFound();
            }

            return actionItem;
        }

        // PUT: api/actionitems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActionItem(int id, ActionItem actionItem)
        {
            if (id != actionItem.ActionItemId)
            {
                return BadRequest();
            }

            _context.Entry(actionItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionItemExists(id))
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

        // POST: api/actionitems
        [HttpPost]
        public async Task<ActionResult<ActionItem>> PostActionItem(ActionItem actionItem)
        {
            _context.ActionItems.Add(actionItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActionItem", new { id = actionItem.ActionItemId }, actionItem);
        }

        // DELETE: api/actionitems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActionItem(int id)
        {
            var actionItem = await _context.ActionItems.FindAsync(id);
            if (actionItem == null)
            {
                return NotFound();
            }

            _context.ActionItems.Remove(actionItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActionItemExists(int id)
        {
            return _context.ActionItems.Any(e => e.ActionItemId == id);
        }
    }
}
