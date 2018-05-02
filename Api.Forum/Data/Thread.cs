using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Forum.Data
{
	/// <summary>
	/// Forum thread/topic. Root node for its replies.
	/// </summary>
	/// <remarks>
	/// Because TPC is still not implemented in EF Core ignore the duplicate attributes, such as Id, UserCreated ...
	/// https://github.com/aspnet/EntityFrameworkCore/issues/3170
	/// </remarks>
	[Table("Threads")]
	public class Thread
	{
		/// <summary>
		/// Thread Id
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
		/// Subject (header) of a thread.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Text body of thread post.
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Replies to the thread.
		/// </summary>
		public List<ThreadReply> Replies { get; set; }

		/// <summary>
		/// Count of replies to the thread.
		/// </summary>
		public int RepliesCount { get; set; }
	}
}
