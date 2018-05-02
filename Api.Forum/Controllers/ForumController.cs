using System;
using System.Net;
using System.Threading.Tasks;
using Api.Forum.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.Forum.Controllers
{
	[Authorize("scope:api.forum")]//Not necessary when `defaultPolicy` is enabled
	[Route("api/v1/[controller]")]
    public class ForumController : ControllerBase
	{
	    private readonly ForumDbContext _forumDbContext;
	    private readonly ILogger<ForumController> _logger;

	    public ForumController(ForumDbContext forumDbContext, ILoggerFactory loggerFactory)
	    {
		    _forumDbContext = forumDbContext ?? throw new ArgumentNullException(nameof(forumDbContext));
		    _logger = loggerFactory.CreateLogger<ForumController>();
		}

        // GET api/v1/threads
        [HttpGet]
		[Authorize("role:user")]
        [Route("threads")]
		public async Task<IActionResult> Items()
        {
	        _logger.LogDebug("{Action} request starting.", nameof(Items));
			var threads = await _forumDbContext.Threads.AsNoTracking().ToListAsync();
	        return Ok(threads);
        }

		//GET api/v1/[controller]/threads/id
		[HttpGet]
	    [Route("threads/{id:int}")]
		[Authorize("role:user")]
	    [ProducesResponseType((int)HttpStatusCode.NotFound)]
	    [ProducesResponseType(typeof(Thread), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetItemById([FromRoute]int id)
        {
	        _logger.LogDebug("{Action} request starting.", nameof(GetItemById));

			if (id <= 0)
	        {
		        return BadRequest();
	        }

	        var item = await _forumDbContext.Threads.SingleOrDefaultAsync(ci => ci.Id == id);
	        if (item != null)
	        {
		        return Ok(item);
	        }

	        return NotFound();
		}

		//GET api/v1/[controller]/threads/view/id
		[HttpGet]
		[Route("threads/view/{id:int}")]
		[Authorize("role:user")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Thread), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetThreadViewById([FromRoute]int id)
		{
			_logger.LogDebug("{Action} request starting.", nameof(GetThreadViewById));

			if (id <= 0)
			{
				return BadRequest();
			}

			var item = await _forumDbContext.
				Threads
				.Include(t=>t.Replies)
				.SingleOrDefaultAsync(ci => ci.Id == id);
			if (item != null)
			{
				return Ok(item);
			}

			return NotFound();
		}

		//POST api/v1/[controller]/threads
		[Route("threads")]
		[Authorize("role:moderator")]
		[HttpPost]
	    [ProducesResponseType((int)HttpStatusCode.Created)]
	    public async Task<IActionResult> CreateItem([FromBody]Thread thread)
	    {
		    _logger.LogDebug("{Action} request starting.", nameof(CreateItem));

			thread.TimeCreated = DateTime.UtcNow;
			_forumDbContext.Threads.Add(thread);
		    await _forumDbContext.SaveChangesAsync();

			return CreatedAtAction("GetItemById", new { id = thread.Id }, thread);
	    }

		//POST api/v1/[controller]/threads
		[Route("threads")]
		[Authorize("role:moderator")]
		[HttpPut]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		public async Task<IActionResult> UpdateItem([FromBody]Thread thread)
		{
			_logger.LogDebug("{Action} request starting.", nameof(UpdateItem));

			if (thread.Id <= 0)
			{
				return BadRequest();
			}

			var threadExisting = await _forumDbContext.Threads.SingleOrDefaultAsync(ci => ci.Id == thread.Id);
			threadExisting.Subject = thread.Subject;
			threadExisting.Content = thread.Content;
			_forumDbContext.Threads.Update(threadExisting);
			await _forumDbContext.SaveChangesAsync();

			return CreatedAtAction("GetItemById", new { id = thread.Id }, thread);
		}

		//POST api/v1/[controller]/threads/reply
		[Route("threads/reply")]
		[Authorize("role:user")]
		[HttpPost]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		public async Task<IActionResult> CreateReply([FromBody]ThreadReply reply)
		{
			_logger.LogDebug("{Action} request starting.", nameof(CreateReply));

			if (reply.ThreadId <= 0)
			{
				return BadRequest();
			}

			using (var transaction = _forumDbContext.Database.BeginTransaction())
			{
				reply.TimeCreated = DateTime.UtcNow;
				_forumDbContext.ThreadReplies.Add(reply);

				var thread = await _forumDbContext.Threads.SingleOrDefaultAsync(ci => ci.Id == reply.ThreadId);
				thread.RepliesCount++;
				_forumDbContext.Threads.Update(thread);

				await _forumDbContext.SaveChangesAsync();
				transaction.Commit();
				return Ok(reply);
			}
		}

		//DELETE api/v1/[controller]/threads/id
		[Route("threads/{id:int}")]
		[HttpDelete]
		[Authorize("role:admin")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> DeleteThread(int id)
		{
			_logger.LogDebug("{Action} request starting.", nameof(DeleteThread));

			if (id <= 0)
			{
				return BadRequest();
			}

			var thread = await _forumDbContext.Threads.SingleOrDefaultAsync(x => x.Id == id);
			if (thread == null)
			{
				return NotFound();
			}

			_forumDbContext.Threads.Remove(thread);
			await _forumDbContext.SaveChangesAsync();
			return NoContent();
		}
	}
}
