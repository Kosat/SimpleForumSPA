using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.Forum.Data
{
	public class ForumDbContext : DbContext
	{
		public DbSet<Thread> Threads { get; set; }
		public DbSet<ThreadReply> ThreadReplies { get; set; }

		public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
		{
		}
	}
}