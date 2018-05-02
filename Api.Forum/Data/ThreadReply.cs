using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Forum.Data
{
	[Table("ThreadReplies")]
	public class ThreadReply
	{
		/// <summary>
		/// Reply Id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Creation time
		/// </summary>
		public DateTime TimeCreated { get; set; }

		/// <summary>
		/// User Id that created thread
		/// </summary>
		public string UserCreatedId { get; set; }

		/// <summary>
		/// User Email that created thread
		/// </summary>
		public string UserCreatedEmail { get; set; }

		/// <summary>
		/// Related Thread Id [FK]
		/// </summary>
		public int ThreadId { get; set; }

		/// <summary>
		/// Text body of reply post.
		/// </summary>
		public string Content { get; set; }
	}
}